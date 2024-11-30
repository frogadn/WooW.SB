using System.IO;
using WooW.SB.Class;
using WooW.SB.Config;

namespace WooW.SB.Helpers
{
    public static class FormateaTodo
    {
        public static void Do()
        {
            Proyecto proyecto = Proyecto.getInstance();

            foreach (
                string file in Directory.GetFiles(
                    proyecto.DirProyectDataLogicPostConditions,
                    "*.cs",
                    SearchOption.AllDirectories
                )
            )
                File.WriteAllText(file, SyntaxEditorHelper.PrettyPrint(File.ReadAllText(file)));

            foreach (
                string file in Directory.GetFiles(
                    proyecto.DirProyectDataLogicPreConditions,
                    "*.cs",
                    SearchOption.AllDirectories
                )
            )
                File.WriteAllText(file, SyntaxEditorHelper.PrettyPrint(File.ReadAllText(file)));

            foreach (
                string file in Directory.GetFiles(
                    proyecto.DirProyectDataLogicModelPoliza,
                    "*.cs",
                    SearchOption.AllDirectories
                )
            )
                File.WriteAllText(file, SyntaxEditorHelper.PrettyPrint(File.ReadAllText(file)));

            foreach (
                string file in Directory.GetFiles(
                    proyecto.DirProyectDataLogicModelCreation,
                    "*.cs",
                    SearchOption.AllDirectories
                )
            )
                File.WriteAllText(file, SyntaxEditorHelper.PrettyPrint(File.ReadAllText(file)));

            foreach (
                string file in Directory.GetFiles(
                    proyecto.DirProyectDataLogicScripts,
                    "*.cs",
                    SearchOption.AllDirectories
                )
            )
                File.WriteAllText(file, SyntaxEditorHelper.PrettyPrint(File.ReadAllText(file)));

            foreach (
                string file in Directory.GetFiles(
                    proyecto.DirProyectData_Test_UnitTest,
                    "*.cs",
                    SearchOption.AllDirectories
                )
            )
                File.WriteAllText(file, SyntaxEditorHelper.PrettyPrint(File.ReadAllText(file)));
        }
    }
}
