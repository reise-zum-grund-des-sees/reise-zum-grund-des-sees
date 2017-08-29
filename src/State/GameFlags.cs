using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
	[Flags]
	enum GameFlags
	{
		Menu = 1,
		GameLoaded = 2,
		GameRunning = 4,
		EditorMode = 8,
        Debug = 16,
        Fullscreen = 32,
        Credits = 64
	}
}
