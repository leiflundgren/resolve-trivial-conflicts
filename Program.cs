namespace resolve_trivial_conflicts
{
    internal class Program
    {
        static void Main(string[] args)
        {

            TextReader reader = ( args.Length == 0 ) ? Console.In :  File.OpenText( args[0 ] );
            TextWriter writer = Console.Out;
            var file = new ConflictFile(reader, writer);

            file.ResolveSimple();
        }
    }
}
