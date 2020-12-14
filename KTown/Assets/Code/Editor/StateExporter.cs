using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;
using UnityEngine;
using States;

class StateExporter
{
    [MenuItem("KTown/Export States")]
    public static void ExportAllStates()
    {
        string folder = $"{Application.dataPath}/Data";
        string file = $"{folder}/states.csv";
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        using (var reader = new StreamWriter(file))
        {
            StringBuilder sb = new StringBuilder();
            foreach (var v in GameObject.FindObjectsOfType<BasicState>())
            {
                
            }
        }
    }
}