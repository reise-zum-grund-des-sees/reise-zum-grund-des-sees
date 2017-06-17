# Neues Dateiformat

Um States besser zu speichern wurde ein neues, auf XML-basierendes Dateiformat eingeführt.
Die Rohdaten der Welt sollten aber identisch sein.
Um eine alte Welt auf das neue Format umzustellen braucht man also nur eine neue Welt speichern und die state.conf in die vorhandene Welt kopieren (überschreiben).

# Steuerung

| Key               | Effect                     |
|-------------------|----------------------------|
| Strg+E            | Toggle Welt-Editor-Mode    |
| Strg+S            | Save World                 |
| Strg+L            | Load World                 |
| Q                 | Down                       |
| E                 | Up                         |
| Left Mouse Click  | Set Block                  |
| Right Mouse Click | Remove Block               |

# Blender

Um in Blender Objekte in die richtige Größe zu bekommen muss man 'n' drücken, um die Eigenschaften angezeigt zu bekommen und nach dem Ändern Strg+A drücken und Position, Skalierung und Rotation anwenden.
Damit die Achsen passen, muss das Model für Monogame um -90° auf der X-Achse und 180° auf der Y-Achse gedreht werden.

# Technical Design Document

https://docs.google.com/document/d/1SUtCXHZ9y6g_IjEutJ3WfhtrATHrZMJoUDgo4YOMOL4/edit
