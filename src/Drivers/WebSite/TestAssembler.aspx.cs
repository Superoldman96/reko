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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Loading;
using System;
using System.ComponentModel.Design;

namespace Reko.WebSite
{
    public class TestAssembler // : System.Web.UI.Page
	{
#if WEB_FORMS
		private WebDecompilerHost host;

		protected System.Web.UI.WebControls.DropDownList ddlSamples;
		protected System.Web.UI.WebControls.TextBox txtAssembler;
		protected System.Web.UI.WebControls.Button btnDecompile;
		protected System.Web.UI.WebControls.Literal plcDecompiled;
		protected System.Web.UI.WebControls.Literal plcOutput;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			host = new WebDecompilerHost();
			if (!IsPostBack)
			{
				host.PopulateSampleFiles(Server, "*.asm", ddlSamples);
			}
		}

        #region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.ddlSamples.SelectedIndexChanged += new System.EventHandler(this.ddlSamples_SelectedIndexChanged);
			this.btnDecompile.Click += new System.EventHandler(this.btnDecompile_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
        #endregion

		private void btnDecompile_Click(object sender, System.EventArgs e)
		{
            try
            {
                DecompileAssembler("x86-protected-32", null);
            } 
            catch
            {
                throw;
            }
		}

		private void DecompileAssembler(string archName, Address loadAddress)
		{
            var sc = new ServiceContainer();
            var cfg = RekoConfigurationService.Load(sc);
            var arch = cfg.GetArchitecture(archName);
            var asm = arch.CreateAssembler(null);
            var program = asm.AssembleFragment(loadAddress, txtAssembler.Text + Environment.NewLine);
            var loader = new Loader(sc);
            var decomp = new Decompiler(loader, sc);
            var proj = new Project {
                Programs = {
                    program
                }
            };
			decomp.Project = proj;
            decomp.ScanPrograms();
            decomp.AnalyzeDataFlow();
            decomp.ReconstructTypes();
            decomp.StructureProgram();
            decomp.WriteDecompilerProducts();

			plcOutput.Text = host.DisassemblyWriter.ToString();
			plcDecompiled.Text = host.DecompiledCodeWriter.ToString();

		}

		private void ddlSamples_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtAssembler.Text = host.FetchSample(Server, ddlSamples.SelectedValue);
		}
#endif
    }
}
