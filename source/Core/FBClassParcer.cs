using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FB2SMV.FBCollections;
using FB2SMV.FBXML;
using FB2SMV.ST;

namespace FB2SMV
{
    namespace Core
    {
        public class FBClassParcer
        {
            private List<string> fileExtensions = new List<string>(new[] {".fbt", ".xml"});

            public FBClassParcer()
            {
                Storage = new FB2SMV.FBCollections.Storage();
                _newTypes = new Queue<string>();
                _processedTypes = new SortedSet<string>();
            }
            public FBClassParcer(Storage openedStorage)
            {
                Storage = openedStorage;
                _newTypes = null;
                _processedTypes = null;
            }

            public void ParseRecursive(string filename)
            {
                if (!fileExtensions.Contains(Path.GetExtension(filename)))
                    fileExtensions.Add(Path.GetExtension(filename));

                string directory = Path.GetDirectoryName(filename);
                bool elementIsRoot = true;
                while (filename != null)
                {
                    try //TODO: replace with FaultCallbackDelegate()
                    {

                        FB2SMV.FBXML.FBType fb = Deserialie(filename);
                        PutClassToStorage(fb, elementIsRoot);
                        filename = NextFileName(directory);
                        elementIsRoot = false;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(String.Format("Can not load file \"{0}\" \n{1}", filename, e.Message));
                    }
                }

            }

            private FB2SMV.FBXML.FBType Deserialie(string filename)
            {
                XmlSerializer ser = new XmlSerializer(typeof (FB2SMV.FBXML.FBType), "");
                StreamReader sr = new StreamReader(filename);
                FB2SMV.FBXML.FBType fb = (FB2SMV.FBXML.FBType) ser.Deserialize(sr);
                sr.Close();
                return fb;
            }

            private string NextFileName(string directory)
            {
                if (_newTypes.Count > 0)
                {
                    string fbType = _newTypes.Dequeue();
                    foreach (var extension in fileExtensions)
                    {
                        string filename = Path.Combine(directory, fbType + extension);
                        if (File.Exists(filename)) return filename;
                    }
                    throw new FileNotFoundException(
                        String.Format("Definition of type \"{0}\" not found in directory \"{1}\" ", fbType, directory));
                }
                return null;
                //return NewTypes.Count > 0 ? Path.Combine()/*directory + NewTypes.Dequeue()*/ : null;
            }

            public void PutClassToStorage(FB2SMV.FBXML.FBType fbType, bool elementIsRoot)
            {
                FB2SMV.FBCollections.FBClass type;
                if (fbType.BasicFB != null) type = FBClass.Basic;
                else if (fbType.FBNetwork != null) type = FBClass.Composite;
                else
                    throw new Exception(
                        String.Format("Unknown FB Type {0}. Only Basic and Composite FB's are supported.", fbType.Name));

                Storage.PutFBType(new FB2SMV.FBCollections.FBType(fbType.Name, fbType.Comment, type, elementIsRoot));
                PutInterfaces(fbType.InterfaceList, fbType.Name);

                if (fbType.BasicFB != null) //Basic FB
                {
                    PutBasicFB(fbType.BasicFB, fbType.Name);
                }
                else if (fbType.FBNetwork != null)
                {
                    PutFBNetwork(fbType.FBNetwork, fbType.Name);
                }

                _processedTypes.Add(fbType.Name);
            }

            private void PutInterfaces(FB2SMV.FBXML.InterfaceList interfaceList, string fbTypeName)
            {
                foreach (var eventInput in interfaceList.EventInputs)
                {
                    Storage.PutEvent(new FB2SMV.FBCollections.Event(eventInput.Name, eventInput.Comment, fbTypeName,
                        Direction.Input));
                    foreach (var withConnection in eventInput.With)
                    {
                        Storage.PutWithConnection(new FB2SMV.FBCollections.WithConnection(fbTypeName, eventInput.Name,
                            withConnection.Var));
                    }
                }
                foreach (var eventOutput in interfaceList.EventOutputs)
                {
                    Storage.PutEvent(new FB2SMV.FBCollections.Event(eventOutput.Name, eventOutput.Comment, fbTypeName,
                        Direction.Output));
                    foreach (var withConnection in eventOutput.With)
                    {
                        Storage.PutWithConnection(new FB2SMV.FBCollections.WithConnection(fbTypeName, eventOutput.Name,
                            withConnection.Var));
                    }

                }
                foreach (var inputVar in interfaceList.InputVars)
                {
                    Storage.PutVariable(new FB2SMV.FBCollections.Variable(inputVar.Name, inputVar.Comment, fbTypeName,
                        Direction.Input, inputVar.Type, inputVar.ArraySize, inputVar.InitialValue, Smv.DataTypes.GetType(inputVar.Type)));
                }
                foreach (var outputVar in interfaceList.OutputVars)
                {
                    Storage.PutVariable(new FB2SMV.FBCollections.Variable(outputVar.Name, outputVar.Comment, fbTypeName,
                        Direction.Output, outputVar.Type, outputVar.ArraySize, outputVar.InitialValue, Smv.DataTypes.GetType(outputVar.Type)));
                }
            }

            private void PutFBNetwork(FBNetwork fbNetwork, string fbTypeName)
            {
                foreach (var fbInstance in fbNetwork.FB)
                {
                    Storage.PutFBInstance(new FB2SMV.FBCollections.FBInstance(fbInstance.Name, fbInstance.Type, fbInstance.Comment, fbTypeName));
                    fbInstance.Parameters.ForEach(p => Storage.PutInstanceParameter(new FBCollections.InstanceParameter(p.Name, p.Value, fbInstance.Name, fbTypeName, p.Comment)));

                    if (!_processedTypes.Contains(fbInstance.Type) && !_newTypes.Contains(fbInstance.Type))
                        _newTypes.Enqueue(fbInstance.Type);

                }
                foreach (var connection in fbNetwork.DataConnections)
                {
                    Storage.PutConnection(new FB2SMV.FBCollections.Connection(connection.Source, connection.Destination,
                        ConnectionType.Data, fbTypeName));
                }
                foreach (var connection in fbNetwork.EventConnections)
                {
                    Storage.PutConnection(new FB2SMV.FBCollections.Connection(connection.Source, connection.Destination,
                        ConnectionType.Event, fbTypeName));
                }

            }

            private void PutBasicFB(BasicFB basicFb, string fbTypeName)
            {
                foreach (var internalVar in basicFb.InternalVars)
                {
                    Storage.PutVariable(new FB2SMV.FBCollections.Variable(internalVar.Name, internalVar.Comment,
                        fbTypeName, Direction.Internal, internalVar.Type, internalVar.ArraySize,
                        internalVar.InitialValue, Smv.DataTypes.GetType(internalVar.Type)));
                }
                foreach (var ecState in basicFb.ECC.ECState)
                {
                    int actionCounter = 0;
                    foreach (var ecAction in ecState.ECAction)
                    {
                        Storage.PutECAction(new FB2SMV.FBCollections.ECAction(fbTypeName, ++actionCounter, ecState.Name,
                            ecAction.Algorithm, ecAction.Output));
                    }
                    Storage.PutState(new FB2SMV.FBCollections.ECState(ecState.Name, ecState.Comment, fbTypeName,
                        actionCounter));
                }
                foreach (var ecTransition in basicFb.ECC.ECTransition)
                {
                    Storage.PutECTransition(new FB2SMV.FBCollections.ECTransition(fbTypeName, ecTransition.Source,
                        ecTransition.Destination, ecTransition.Condition));
                }
                foreach (var algorithm in basicFb.Algorithms)
                {
                    Storage.PutAlgorithm(new FB2SMV.FBCollections.Algorithm(algorithm.Name, algorithm.Comment,
                        fbTypeName, AlgorithmLanguages.ST, algorithm.ST.Text));
                    foreach (OutputLine line in FB2SMV.ST.Translator.Translate(algorithm.ST.Text))
                    {
                        Storage.PutAlgorithmLine(new AlgorithmLine(line.NI,
                            line.Variable,
                            line.Condition,
                            line.Value,
                            fbTypeName,
                            algorithm.Name));
                    }

                }
            }

            public readonly Storage Storage;
            private Queue<string> _newTypes;
            private SortedSet<string> _processedTypes;

        }
    }
}