using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using Syroot.NintenTools.NSW.Bfres.Core;

namespace Syroot.NintenTools.NSW.Bfres;

/// <summary>
/// Represents the non-generic base of a dictionary which can quickly look up <see cref="T:Syroot.NintenTools.NSW.Bfres.Core.IResData" /> instances via
/// key or index.
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(TypeProxy))]
public class ResDict : IEnumerable, IResData
{
	private class Tree
	{
		public Node root;

		public Dictionary<BigInteger, Tuple<int, Node>> entries;

		public Tree()
		{
			entries = new Dictionary<BigInteger, Tuple<int, Node>>();
			root = new Node(0, -1, root);
			root.Parent = root;
			insertEntry(0, root);
		}

		private int GetCompactBitIdx()
		{
			return -1;
		}

		public void insertEntry(BigInteger data, Node node)
		{
			entries[data] = Tuple.Create(entries.Count, node);
		}

		private Node Search(BigInteger data, bool prev)
		{
			if (root.Child[0] == root)
			{
				return root;
			}
			Node node = root.Child[0];
			Node node2 = node;
			do
			{
				node2 = node;
				node = node.Child[_bit(data, node.bitInx)];
			}
			while (node.bitInx > node2.bitInx);
			if (prev)
			{
				return node2;
			}
			return node;
		}

		public void Insert(string Name)
		{
			string source = ToBinaryString(Name, Encoding.UTF8);
			BigInteger bigInteger = source.Aggregate(default, (BigInteger b, char c) => b * 2 + c - (ushort)48);
			Node node = Search(bigInteger, prev: true);
			int num = bit_mismatch(node.Data, bigInteger);
			while (num < node.Parent.bitInx)
			{
				node = node.Parent;
			}
			if (num < node.bitInx)
			{
				Node node2 = new Node(bigInteger, num, node.Parent);
				node2.Child[_bit(bigInteger, num) ^ 1] = node;
				node.Parent.Child[_bit(bigInteger, node.Parent.bitInx)] = node2;
				node.Parent = node2;
				insertEntry(bigInteger, node2);
				return;
			}
			if (num > node.bitInx)
			{
				Node node3 = new Node(bigInteger, num, node);
				if (_bit(node.Data, num) == (_bit(bigInteger, num) ^ 1))
				{
					node3.Child[_bit(bigInteger, num) ^ 1] = node;
				}
				else
				{
					node3.Child[_bit(bigInteger, num) ^ 1] = root;
				}
				node.Child[_bit(bigInteger, node.bitInx)] = node3;
				insertEntry(bigInteger, node3);
				return;
			}
			int num2 = first_1bit(bigInteger);
			if (node.Child[_bit(bigInteger, num)] != root)
			{
				num2 = bit_mismatch(node.Child[_bit(bigInteger, num)].Data, bigInteger);
			}
			Node node4 = new Node(bigInteger, num2, node);
			node4.Child[_bit(bigInteger, num2) ^ 1] = node.Child[_bit(bigInteger, num)];
			node.Child[_bit(bigInteger, num)] = node4;
			insertEntry(bigInteger, node4);
		}
	}

	/// <summary>
	/// Represents a node forming the Patricia trie of the dictionary.
	/// </summary>
	[DebuggerDisplay("Node {Key}")]
	protected class Node
	{
		internal const int SizeInBytes = 16;

		internal List<Node> Child = new List<Node>();

		internal Node Parent;

		internal int bitInx;

		internal BigInteger Data;

		internal uint Reference;

		internal ushort IdxLeft;

		internal ushort IdxRight;

		internal string Key;

		internal Node()
		{
			Child.Add(this);
			Child.Add(this);
			Reference = uint.MaxValue;
		}

		internal string GetName()
		{
			BigInteger bigInteger = BitLength(Data);
			byte[] array = Data.ToByteArray();
			Array.Reverse(array, 0, array.Length);
			return Encoding.UTF8.GetString(array);
		}

		internal int GetCompactBitIdx()
		{
			int num = bitInx / 8;
			return (num << 3) | (bitInx - 8 * num);
		}

		internal Node(BigInteger data, int bitidx, Node parent)
			: this()
		{
			Data = data;
			bitInx = bitidx;
			Parent = parent;
		}

		internal Node(string key)
			: this()
		{
			Key = key;
		}
	}

	private class TypeProxy
	{
		private ResDict _dict;

		internal TypeProxy(ResDict dict)
		{
			_dict = dict;
		}
	}

	private IList<Node> _nodes;

	/// <summary>
	/// Gets the number of instances stored.
	/// </summary>
	public int Count => _nodes.Count - 1;

	/// <summary>
	/// Returns only the publically visible nodes, excluding the root node.
	/// </summary>
	protected IEnumerable<Node> Nodes
	{
		get
		{
			for (int i = 1; i < _nodes.Count; i++)
			{
				yield return _nodes[i];
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Syroot.NintenTools.NSW.Bfres.ResDict" /> class.
	/// </summary>
	public ResDict()
	{
		_nodes = new List<Node>
		{
			new Node()
		};
	}

	/// <summary>
	/// Adds the given <paramref name="key" /> to insert in the dictionary.
	/// </summary>
	/// <exception cref="T:System.ArgumentException">Duplicated <paramref name="key" /> instances
	/// already exists.</exception>
	public void Add(string key)
	{
		if (!ContainsKey(key))
		{
			_nodes.Add(new Node(key));
			return;
		}
		throw new Exception("key " + key + " already exists in the dictionary!");
	}

	/// <summary>
	/// Removes the given <paramref name="key" /> from the dictionary.
	/// </summary>
	/// <exception cref="T:System.ArgumentException">Duplicated <paramref name="key" /> instances
	/// already exists.</exception>
	public void Remove(string key)
	{
		_nodes.Remove(_nodes.Where((Node n) => n.Key == key).FirstOrDefault());
	}

	/// <summary>
	/// Determines whether the given <paramref name="key" /> is in the dictionary.
	/// </summary>
	/// <returns><c>true</c> if <paramref name="key" /> was found in the dictionary; otherwise <c>false</c>.
	/// </returns>
	public bool ContainsKey(string key)
	{
		return _nodes.Any((Node p) => p.Key == key);
	}

	/// <summary>
	/// Returns the key given <paramref name="index" /> is within range of the dictionary.
	/// </summary>
	public string GetKey(int index)
	{
		if (index < _nodes.Count || index > 0)
		{
			return _nodes[index + 1].Key;
		}
		throw new Exception($"Index {index} is out of range!");
	}

	public int GetIndex(string key)
	{
		int num = _nodes.SkipWhile((Node p) => p.Key == "key").Count();
		if (num < _nodes.Count || num > 0)
		{
			return num;
		}
		return -1;
	}

	/// <summary>
	/// Removes all elements from the dictionary.
	/// </summary>
	public void Clear()
	{
		_nodes.Clear();
		_nodes.Add(new Node());
	}

	void IResData.Load(ResFileLoader loader)
	{
		uint num = loader.ReadUInt32();
		int num2 = loader.ReadInt32();
		int num3 = 0;
		List<Node> list = new List<Node>();
		while (num2 >= 0)
		{
			list.Add(ReadNode(loader));
			num3++;
			num2--;
		}
		_nodes = list;
	}

	void IResData.Save(ResFileSaver saver)
	{
		UpdateNodes();
		saver.Write(0);
		saver.Write(Count);
		int num = -1;
		int num2 = 0;
		foreach (Node node in _nodes)
		{
			saver.Write(node.Reference);
			saver.Write(node.IdxLeft);
			saver.Write(node.IdxRight);
			if (num2 == 0)
			{
				saver.SaveRelocateEntryToSection(saver.Position, 1u, (uint)_nodes.Count, 1u, 1, "");
				saver.SaveString("");
			}
			else
			{
				saver.SaveString(node.Key);
			}
			num2++;
		}
	}

	private IEnumerator<string> GetEnumerator()
	{
		foreach (Node node in Nodes)
		{
			yield return node.Key;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private static string ToBinaryString(string text, Encoding encoding)
	{
		return string.Join("", from n in encoding.GetBytes(text)
			select Convert.ToString(n, 2).PadLeft(8, '0'));
	}

	private static int _bit(BigInteger n, int b)
	{
		BigInteger bigInteger = (n >> (int)(b & 0xFFFFFFFFu)) & 1;
		return (int)bigInteger;
	}

	private static int first_1bit(BigInteger n)
	{
		int num = BitLength(n);
		for (int i = 0; i < num; i++)
		{
			if (((n >> i) & 1) == 1L)
			{
				return i;
			}
		}
		throw new Exception("Operation Failed");
	}

	private static int bit_mismatch(BigInteger int1, BigInteger int2)
	{
		int val = BitLength(int1);
		int val2 = BitLength(int2);
		for (int i = 0; i < Math.Max(val, val2); i++)
		{
			if (((int1 >> i) & 1) != ((int2 >> i) & 1))
			{
				return i;
			}
		}
		return -1;
	}

	private static int BitLength(BigInteger bits)
	{
		int num = 0;
		while (bits / 2 != 0L)
		{
			bits /= (BigInteger)2;
			num++;
		}
		return num + 1;
	}

	private void UpdateNodes()
	{
		Tree tree = new Tree();
		_nodes[0] = new Node
		{
			Key = string.Empty,
			bitInx = -1,
			Parent = _nodes[0]
		};
		for (ushort num = 1; num < _nodes.Count; num++)
		{
			tree.Insert(_nodes[num].Key);
		}
		int num2 = 0;
		foreach (Tuple<int, Node> value in tree.entries.Values)
		{
			Node item = value.Item2;
			item.Reference = (uint)(item.GetCompactBitIdx() & 0xFFFFFFFFu);
			item.IdxLeft = (ushort)tree.entries[item.Child[0].Data].Item1;
			item.IdxRight = (ushort)tree.entries[item.Child[1].Data].Item1;
			item.Key = item.GetName();
			_nodes[num2] = item;
			num2++;
		}
		_nodes[0].Key = null;
	}

	private Node ReadNode(ResFileLoader loader)
	{
		return new Node
		{
			Reference = loader.ReadUInt32(),
			IdxLeft = loader.ReadUInt16(),
			IdxRight = loader.ReadUInt16(),
			Key = loader.LoadString()
		};
	}
}
