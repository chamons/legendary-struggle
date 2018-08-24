using System;
using LS.Core;

namespace LS.Console
{
    class Program
    {
		public void Run ()
		{
			var v = LS.Core.Configuration.InitialState.LoadDefault ();
		}

        static void Main(string[] args)
        {
			(new Program ()).Run ();
        }
    }
}
