﻿using Veil.Parser.Nodes;

namespace Veil.Compiler
{
    internal partial class VeilTemplateCompiler<T>
    {
        private void EmitBlock(BlockNode node)
        {
            foreach (var n in node.Nodes)
            {
                EmitNode(n);
            }
        }
    }
}