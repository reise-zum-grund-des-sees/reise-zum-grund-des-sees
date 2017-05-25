using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
	delegate void UpdateDelegate(ref GameState gs);

    static class UpdateDelegateHelper
    {
        public static UpdateDelegate ContinueWith(this UpdateDelegate _del1, UpdateDelegate _del2)
            => (ref GameState gs) => { _del1?.Invoke(ref gs); _del2?.Invoke(ref gs); };
    }

	interface IUpdateable
	{
		/// <summary>
		/// Update Delegate
		/// </summary>
		/// <param name="_view">Die aktuelle Version des GameStates</param>
		/// <param name="_inputArgs">Nutzer-Ereignisse</param>
		/// <param name="_passedTime">Verstichene Zeit seit dem letzten Update</param>
		/// <returns>UpdateDelegate, dass den GameState verändern kann: return (ref GameState _state) => { /* CODE HERE */ };</returns>
		UpdateDelegate Update(GameState.View _view, InputEventArgs _inputArgs, double _passedTime);
	}

}
