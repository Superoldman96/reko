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

using Reko.Core.Services;
using Reko.Gui.Services;
using Reko.Gui.ViewModels;
using System;

namespace Reko.Gui
{
    /// <summary>
    /// Base class to use for services that display a document view in the 
    /// user interface.
    /// </summary>
    public class ViewService
    {
        public ViewService(IServiceProvider services)
        {
            this.Services = services;
        }

        public IServiceProvider Services { get; }

        protected IWindowFrame ShowWindow(
            string windowType,
            string windowTitle,
            object docItem,
            Func<IWindowPane> paneMaker)
        {
            return ShowWindow(windowType, windowTitle, docItem, paneMaker());
        }

        protected IWindowFrame ShowWindow(
            string windowType,
            string windowTitle,
            object docItem,
            IWindowPane innerPane)
        {
            var shellUiSvc = Services.RequireService<IDecompilerShellUiService>();
            var windowFrame = shellUiSvc.FindDocumentWindow(windowType, docItem);
            if (windowFrame is null)
            {
                windowFrame = shellUiSvc.CreateDocumentWindow(windowType, docItem, windowTitle, innerPane);
            }
            windowFrame.Show();
            return windowFrame;
        }
    }
}
