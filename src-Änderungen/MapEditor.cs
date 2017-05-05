using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace ReiseZumGrundDesSees
{
    class MapEditor
    {
        int[,,] map;
        string textfile;
        string[] lines;
        public MapEditor(string file)
        {
            map = new int[20, 20, 10];
            textfile = file;
            lines = System.IO.File.ReadAllLines(textfile); //read file  
  
         
        }

        public void readtxt()
        {
            int Spalte = 0; //momentan gelesene Spalte
            int Zeile = 0; //momentan gelesene Zeile
            int Höhe = 0;
          
            foreach (string line in lines) // lese jede Zeile
            {
              
                Spalte = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (Char.IsWhiteSpace(line[i])) Spalte++; // wenn leerzeichen, dann nächste Zahl
                    else if (i != line.Length - 1 && Char.IsDigit(line[i + 1]))
                        map[Zeile, Spalte, Höhe] = int.Parse(line[i].ToString() + line[i + 1].ToString()); //Verbinde zwei hintereinanderfolgende Zahlen
                    else if (Char.IsDigit(line[i]))
                        map[Zeile, Spalte, Höhe] = (int)char.GetNumericValue(line[i]); //Wenn Zahl, dann schreib ins Array                
                   else if (char.IsPunctuation(line[i])) Höhe++;
                }
                Zeile++;
            }
          
        }

        public void insertBlock(Vector3 pos)
        {
            //Annahme: BlockDurchmesser=10 
            int BlockDurchmesser = 10;
            //Koordinaten des Spielers in Arrayparameter
            int a = (int)pos.X / BlockDurchmesser;
            int b = (int)pos.Y / BlockDurchmesser;
            int c = (int)pos.Z / BlockDurchmesser;
  
            //momentane Parameter
            int a1 = 0;
            int b1 = 0;
            int c1 = 0;
            int linecounter = 0;
            bool gefunden = false;
            foreach (string line in lines) // über alle Zeilen
            {
            
                if (gefunden == false)
                {
                    if (c1 != c)
                    { //Richtige Höhe
                        for (int i = 0; i < line.Length; i++) //über Zeilenlänge
                        
                            if (char.IsPunctuation(line[i])) c1++;



                      
                    }
                    else
                    {

                        if (b1 != b) //Richtige Zeile
                        {
                            b1++;
                        }
                        else
                        {
                            // for (int i = 0; i < line.Length; i++) //über Zeilenlänge

                            char[] s = line.ToCharArray(); ;//neue Zeile
                            
                            for (int i = 0; i <s.Length; i++)
                            {
                                if (a1 == a && Char.IsDigit(s[i]))
                                {
                                    s[i] = '1';
                                    if (i != line.Length - 1 && Char.IsDigit(s[i + 1]) == true) //für Zahlen länge 2
                                        s[i + 1] = ' ';
                                }
                                if (Char.IsDigit(s[i]) && i != line.Length - 1 && Char.IsDigit(s[i+1])==false )
                                    a1++; //finde richtige Spalte
                            
                            }
                            lines[linecounter] = new string(s);//neue Zeile zuweisen
                                 
                                    gefunden = true;
                                  
                                
                            
                        }
                    }
                   
                }
                linecounter++;


            }
         File.WriteAllLines(textfile, lines);
            readtxt();
        }


        }
}