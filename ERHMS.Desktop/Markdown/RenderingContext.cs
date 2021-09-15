using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace ERHMS.Desktop.Markdown
{
    public class RenderingContext
    {
        public IEnumerable<BlockRenderer> BlockRenderers { get; }
        public IEnumerable<InlineRenderer> InlineRenderers { get; }
        public object Heading1StyleKey { get; set; }
        public object Heading2StyleKey { get; set; }
        public object Heading3StyleKey { get; set; }
        public object EmphasizedStyleKey { get; set; }
        public object StrongStyleKey { get; set; }

        public RenderingContext()
        {
            BlockRenderers = BlockRenderer.GetInstances(this).ToList();
            InlineRenderers = InlineRenderer.GetInstances(this).ToList();
        }

        public object GetHeadingStyleKey(int level)
        {
            switch (level)
            {
                case 1:
                    return Heading1StyleKey;
                case 2:
                    return Heading2StyleKey;
                case 3:
                    return Heading3StyleKey;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }

        public IEnumerable<Block> GetBlocks(string text)
        {
            return Regexes.BlockSeparator.Split(text.Trim()).Select(GetBlock);
        }

        private Block GetBlock(string text)
        {
            foreach (BlockRenderer renderer in BlockRenderers)
            {
                if (renderer.TryRender(text, out Block block))
                {
                    return block;
                }
            }
            StringBuilder message = new StringBuilder();
            message.AppendLine("Cannot render text as block.");
            message.Append(text);
            throw new ArgumentException(message.ToString(), nameof(text));
        }

        public IEnumerable<Inline> GetInlines(string text)
        {
            if (text == "")
            {
                return Enumerable.Empty<Inline>();
            }
            foreach (InlineRenderer renderer in InlineRenderers)
            {
                if (renderer.TryRender(text, out InlineNode node))
                {
                    return GetInlines(node);
                }
            }
            StringBuilder message = new StringBuilder();
            message.AppendLine("Cannot render text as inline.");
            message.Append(text);
            throw new ArgumentException(message.ToString(), nameof(text));
        }

        private IEnumerable<Inline> GetInlines(InlineNode node)
        {
            foreach (Inline inline in GetInlines(node.Prefix))
            {
                yield return inline;
            }
            yield return node.Inline;
            foreach (Inline inline in GetInlines(node.Suffix))
            {
                yield return inline;
            }
        }
    }
}
