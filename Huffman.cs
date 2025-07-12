public record Token<T>(T Value, int Frequency)
	where T : notnull;

public static class Huffman<T>
	where T : notnull
{
    class TreeNode
    {
        public T Value { get; set; }
        public int Frequency { get; set; }
        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }
        
        public TreeNode(T value, int frequency)
        {
            Value = value;
            Frequency = frequency;
        }
        
        public TreeNode(TreeNode left, TreeNode right)
        {
            Left = left;
            Right = right;
            Frequency = left.Frequency + right.Frequency;
        }
        
        public bool IsLeaf => Left == null && Right == null;
    }
    
    public static Dictionary<T, BitArray> Build(ReadOnlySpan<Token<T>> tokens)
    {
        if (tokens.Length == 0)
            return new Dictionary<T, BitArray>();
            	
		TreeNode[] leafNodes = new TreeNode[tokens.Length];
		
		for (int i = 0; i < tokens.Length; i++)
		{
			leafNodes[i] = new(tokens[i].Value, tokens[i].Frequency);
		}
			
        TreeNode tree = BuildTree(leafNodes);
                
        Dictionary<T, BitArray> codes = [];
        GenerateCodes(tree, [], ref codes);
        
        return codes;
    }
    
    static TreeNode BuildTree(ReadOnlySpan<TreeNode> nodes)
    {        
        PriorityQueue<TreeNode, int> pq = new();
        
        foreach (TreeNode node in nodes)
        {
            pq.Enqueue(node, node.Frequency);
        }
        
        while (pq.Count > 1)
        {
            TreeNode left = pq.Dequeue();
            TreeNode right = pq.Dequeue();
            
            TreeNode parent = new(left, right);
            pq.Enqueue(parent, parent.Frequency);
        }
        
        return pq.Dequeue();
    }
    
    static void GenerateCodes(TreeNode node, List<bool> currentCode, ref Dictionary<T, BitArray> codes)
    {
        if (node == null)
            return;
                    
        if (node.IsLeaf && node.Value != null)
        {
            codes[node.Value] = new(currentCode.ToArray());
            return;
        }
        
        List<bool> leftCode = new(currentCode) { false };
        List<bool> rightCode = new(currentCode) { true };
		
        GenerateCodes(node.Left, leftCode, ref codes);
        GenerateCodes(node.Right, rightCode, ref codes);
    }
}
