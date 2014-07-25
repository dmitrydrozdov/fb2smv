using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2SMV.FBCollections;
using FB2SMV.ServiceClasses;

namespace FB2SMV
{
    namespace Core
    {
        namespace Structures
        {
            public class TypesTree : SimpleTree<string>
            {
                public TypesTree(){}

                public void Construct(Storage storage)
                {
                    FBType rootFbType = storage.Types.FirstOrDefault(t => t.IsRoot);
                    if (rootFbType == null)
                        throw new Exception();
                    else
                    {
                        Root = new TreeNode<string>(rootFbType.Name);
                    }
                    foreach (FBInstance fbInstance in storage.Instances)
                    {
                        TreeNode<string> parentNode = FindNode(fbInstance.FBType, (a, b) => a == b);
                        if (parentNode == null) throw new Exception();
                        else
                        {
                            TreeNode<string> instTypeNode = parentNode.FindChild(fbInstance.InstanceType, (a, b) => a == b);//FindNode(fbInstance.InstanceType, (a, b) => a == b);
                            if (instTypeNode == null)
                            {
                                parentNode.AppendChild(new TreeNode<string>(fbInstance.InstanceType));
                            }
                        }
                    }
                }
            }
        }
    }
}
