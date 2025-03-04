#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using ReactiveUI;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Loading;
using Reko.Scanning;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Reko.Gui.ViewModels.Documents
{
    /// <summary>
    /// The View model for base address finder document view.
    /// </summary>
    public class BaseAddressFinderViewModel : Reko.Gui.Reactive.ChangeNotifyingObject
    {
        private IServiceProvider services;
        private Program program;
        private readonly string startText;
        private readonly string stopText;
        private CancellationTokenSource? cts;

        public BaseAddressFinderViewModel(
            IServiceProvider services,
            Program program,
            string startText,
            string stopText)
        {
            this.services = services;
            this.program = program;
            this.startText = startText;
            this.stopText = stopText;
            this.startStopButtonText = startText;
            this.startStopButtonEnabled = true;
            this.Results = new ObservableCollection<BaseAddressResult>();
            this.baseAddress = "";
            this.ChangeBaseAddress_Click = ReactiveCommand.Create(ChangeBaseAddress);
        }

        public bool ByString
        {
            get => byString;
            set => this.RaiseAndSetIfChanged(ref byString, value);
        }
        private bool byString;

        public bool ByProlog
        {
            get => byProlog;
            set => this.RaiseAndSetIfChanged(ref byProlog, value);
        }
        private bool byProlog;

        public int ByStringProgress
        {
            get => byStringProgress;
            set => this.RaiseAndSetIfChanged(ref byStringProgress, value);
        }
        private int byStringProgress;

        public int ByPrologProgress
        {
            get => byPrologProgress;
            set => this.RaiseAndSetIfChanged(ref byPrologProgress, value);
        }
        private int byPrologProgress;

        public string StartStopButtonText
        {
            get => startStopButtonText;
            set => this.RaiseAndSetIfChanged(ref startStopButtonText, value);
        }
        private string startStopButtonText;

        public bool StartStopButtonEnabled
        {
            get => startStopButtonEnabled;
            set => this.RaiseAndSetIfChanged(ref startStopButtonEnabled, value);
        }
        private bool startStopButtonEnabled;

        public ObservableCollection<BaseAddressResult> Results { get; }

        public string BaseAddress
        {
            get => baseAddress;
            set
            {
                this.RaiseAndSetIfChanged(ref baseAddress, value);
                ChangeBaseAddressEnabled =
                    this.program.Architecture.TryParseAddress(value, out _);
            }
        }
        public string baseAddress;

        public bool ChangeBaseAddressEnabled
        {
            get => changeBaseAddressEnabled;
            set => this.RaiseAndSetIfChanged(ref changeBaseAddressEnabled, value);
        }
        private bool changeBaseAddressEnabled;

        public ReactiveCommand<Unit, Unit> ChangeBaseAddress_Click { get; }


        public async Task StartStopFinder()
        {
            if (cts is null)
            {
                this.cts = new CancellationTokenSource();
                this.StartStopButtonText = stopText;
                await StartFinder_work(cts.Token);
            }
            else
            {
                // A task is running. Cancel it.
                this.cts.Cancel();
            }
            this.StartStopButtonText = startText;
            this.cts = null;
        }

        private async ValueTask StartFinder_work(CancellationToken token)
        {
            this.Results.Clear();
            if (program.SegmentMap.Segments.Values.FirstOrDefault()?.MemoryArea
                is not ByteMemoryArea mem)
            {
                return;
            }

            var s = new FindBaseString(
                program.Architecture,
                mem,
                NullProgressIndicator.Instance);
            //$REVIEW: the ProcedurePrologFinder is taking way too long.
            //var p = new ProcedurePrologFinder(
            //    program.Architecture,
            //    this.GetApplicablePrologPatterns(program),
            //    mem);

            var sTask = Task.Run(() => s.Run(token));
            //var pTask = Task.Run(s.Run);
            var sResults = await sTask;
            //var pResults = await pTask;

            var arch = program.Architecture;
            PublishResults(arch, sResults, Array.Empty<BaseAddressCandidate>()); // pResults);
        }

        private void PublishResults(
            IProcessorArchitecture arch,
            BaseAddressCandidate[] sResults,
            BaseAddressCandidate[] pResults)
        {
            var addrs = sResults.Concat(pResults).Select(s => s.Address).ToHashSet();
            var results =
                from addr in addrs
                join sr in sResults on addr equals sr.Address into srs
                from sr2 in srs.DefaultIfEmpty()
                join pr in pResults on addr equals pr.Address into prs
                from pr2 in prs.DefaultIfEmpty()
                orderby addr
                select new BaseAddressResult
                {
                    Address = RenderAddress(addr, arch),
                    Confidence = sr2.Confidence != 0
                        ? sr2.Confidence.ToString()
                        : "-",
                };  
            foreach (var result in results)
            {
                this.Results.Add(result);
            }
        }

        private IEnumerable<MaskedPattern> GetApplicablePrologPatterns(Program program)
        {
            var platformPatterns = program.Platform.ProcedurePrologs;
            var archPatterns = program.Platform.Architecture.ProcedurePrologs;
            return platformPatterns.Concat(archPatterns);
        }

        private static string RenderAddress(ulong address, IProcessorArchitecture arch)
        {
            var bitSize = arch.PointerType.BitSize;
            int digits = arch.DefaultBase switch
            {
                16 => (bitSize + 3) / 4,
                8 => (bitSize + 2) / 3,
                _ => throw new NotImplementedException($"Unimplemented bit size {arch.DefaultBase}.")
            };
            var sAddr = Convert.ToString(
                (long) address,
                arch.DefaultBase)
                .ToUpperInvariant();
            return Convert.ToString(sAddr).PadLeft(digits, '0');
        }

        public void ChangeBaseAddress()
        {
            if (!program.Architecture.TryParseAddress(this.BaseAddress, out var addrBase))
                return;
            var loader = services.RequireService<ILoader>();
            var loadDetails = new LoadDetails
            {
                LoadAddress = this.BaseAddress,
                ArchitectureName = program.User.Processor,
                PlatformName = program.User.Environment,
                ArchitectureOptions = program.Architecture.SaveUserOptions(),
                LoaderName = program.User.Loader,
                Location = program.Location,
                // EntryPoint = RebaseEntryPoint(program.User.)
                //Offset = program.User.
            };
            var rebasedProgram = loader.LoadRawImage(loadDetails);
            var dcSvc = services.RequireService<IDecompilerService>();
            dcSvc.Decompiler?.ReplaceProgram(program, rebasedProgram);
            var diagSvc = services.RequireService<IDiagnosticsService>();
            diagSvc.Inform("Base address changed to {0}", addrBase);
        }
    }

    public class BaseAddressResult
    {
        public string? Address { get; set; }

        public string? Confidence { get; set; }
    }
}
