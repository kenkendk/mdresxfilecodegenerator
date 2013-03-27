using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Ide.CustomTools;

namespace ResxDesignerGenerator
{
	public class ResXFileCodeGenerator : MonoDevelop.Ide.CustomTools.ISingleFileCustomTool
	{
		public static void GenerateDesignerFile(string resxfile, string @namespace, string classname, string designerfile)
		{
			var doc = XDocument.Load(resxfile).Root;
			
			foreach(var s in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames())
				Console.WriteLine(s);
			var filetemplate = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ResxDesignerGenerator.HeaderTemplate.txt")).ReadToEnd();
			var elementtemplate = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ResxDesignerGenerator.ElementTemplate.txt")).ReadToEnd();

			var sb = new StringBuilder();
			foreach(var node in from n in doc.Descendants() where n.Name == "data" select n)
			{
				var name = node.Attribute("name").Value;
				var value = node.Descendants().First().Value;
				sb.Append(elementtemplate.Replace("{name}", name).Replace("{value}", System.Web.HttpUtility.HtmlEncode(value.Trim().Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n        ///"))));
			}
				
			using(var w = new StreamWriter(designerfile, false, Encoding.UTF8))
				w.Write(filetemplate.Replace("{runtime-version}", System.Environment.Version.ToString()).Replace("{namespace}", @namespace).Replace("{classname}", classname).Replace("{elementdata}", sb.ToString()));
		}

		internal static string GetNamespaceHint(ProjectFile file, string outputFile)
		{
			string ns = file.CustomToolNamespace;
			if (string.IsNullOrEmpty (ns) && !string.IsNullOrEmpty (outputFile)) {
				var dnp = file.Project as DotNetProject;
					if (dnp != null)
						ns = dnp.GetDefaultNamespace (outputFile);
			}
			return ns;
		}
		
		#region ISingleFileCustomTool implementation

		public IAsyncOperation Generate(IProgressMonitor monitor, ProjectFile file, SingleFileCustomToolResult result)
		{
			return new ThreadAsyncOperation (delegate {
				var outputfile = file.FilePath.ChangeExtension(".Designer.cs");
				var ns = GetNamespaceHint (file, outputfile);
				var cn = file.FilePath.FileNameWithoutExtension;
				
				GenerateDesignerFile(file.FilePath.FullPath, ns, cn, outputfile);
				result.GeneratedFilePath = outputfile;
			}, result);		
		}

		#endregion
	}
}

