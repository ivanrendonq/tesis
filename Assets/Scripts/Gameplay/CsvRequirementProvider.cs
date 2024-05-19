using System;
using System.Collections.Generic;
using System.IO;
using Gameplay.Data;
using UnityEngine;

namespace Gameplay
{
    public class CsvRequirementProvider : RequirementProvider
    {
        private string requirementsFilePath = "";
        private string requirementsFolder = "";

        [HideInInspector]
        public CsvRequirementProvider Instance;

        private void Awake() {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }else{
                Destroy(this);
            }

            requirementsFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Req/requirements.csv";
            requirementsFolder   = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Req";
        }

        public bool ExistFile()
        {
            if(File.Exists(requirementsFilePath))
            {
                Debug.Log("Existe archivo");
                return true;
            }

            Debug.Log("No existe archivo");
            return false;
        }

        public override List<Requirement> GetRequirements()
        {
            List<Requirement> reqs = new List<Requirement>();
            
            using (var reader = new StreamReader(requirementsFilePath))
            {
                while(!reader.EndOfStream)
                {
                    //Reading for lines

                    string line = reader.ReadLine();

                    string[] fields = line.Split(';');
                    
                    //Fields 0 : ambiguity
                    //Fields 1 : description
                    Requirement req = new Requirement();

                    if(fields[0] == "A")
                    {
                        req.requirementType = RequirementType.ambiguous;
                    }else{
                        req.requirementType = RequirementType.noAmbiguous;
                    }

                    req.description = fields[1];

                    reqs.Add(req);                    
                }

            }

            return reqs;
        }

        public bool ValidateFile()
        {
            if(File.Exists(requirementsFilePath) == false)
            {
                Debug.Log("ValidateFile: No existe el archivo");
                return false;
            }

            using (var reader = new StreamReader(requirementsFilePath))
            {
                while(!reader.EndOfStream)
                {
                    //Reading lines
                    string line = reader.ReadLine();
                    string[] fields = line.Split(';');

                    Debug.Log("ValidateFile: Linea: " + line);
                    
                    //Fields 0 : ambiguity
                    //Fields 1 : description

                    if(fields.Length != 2 )
                    {
                        Debug.Log("ValidateFile: No existe dos columnas");
                        return false;
                    }

                    if(fields[0] != "A" && fields[0] != "N")
                    {
                        Debug.Log("ValidateFile: No tiene A o N la fila, tiene: " + fields[0] );
                        return false;
                    }

                    if(fields[1].Trim() == "")
                    {
                        Debug.Log("ValidateFile: El rerquerimiento está vacio");
                        return false;
                    }

                }

            }

            return true;            
        }

        public void CreateFile()
        {
            TextAsset resourceFile = Resources.Load<TextAsset>("requirements");

            if(resourceFile == null)
                return;

            if(Directory.Exists(requirementsFolder) == false)
            {
                Directory.CreateDirectory(requirementsFolder);
            }
            File.WriteAllBytes(requirementsFilePath, resourceFile.bytes);
        }
    }
}