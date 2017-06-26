using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Xml;

namespace ReiseZumGrundDesSees
{
    class ConfigFile
    {
        public readonly Dictionary<string, ConfigNode> Nodes;
        public ConfigNode this[string s] => Nodes[s];

        public ConfigFile(params KeyValuePair<string, ConfigNode>[] _objects)
        {
            Dictionary<string, ConfigNode> _dict = new Dictionary<string, ConfigNode>(_objects.Length);
            _dict.Update(_objects);
        }
        public ConfigFile(Dictionary<string, ConfigNode> _objects)
        {
            Nodes = _objects;
        }
        public ConfigFile()
        {
            Nodes = new Dictionary<string, ConfigNode>();
        }

        public void Update(ConfigFile _configFile)
        {
            foreach (var _obj in _configFile.Nodes)
                Update(_obj.Key, _obj.Value);
        }
        public void Update(string _objName, ConfigNode _object)
        {
            if (Nodes.ContainsKey(_objName))
                Nodes[_objName].Items.Update(_object.Items);
            else
                Nodes[_objName] = _object;
        }

        public class ConfigNode
        {
            public readonly Dictionary<string, string> Items;
            public readonly Dictionary<string, ConfigNode> Nodes;

            public string this[string s] => Items[s];

            public ConfigNode(Dictionary<string, string> _items)
            {
                Items = _items;
                Nodes = new Dictionary<string, ConfigNode>();
            }
            public ConfigNode()
            {
                Items = new Dictionary<string, string>();
                Nodes = new Dictionary<string, ConfigNode>();
            }
            public ConfigNode(XmlNode _xmlNode) : this()
            {
                foreach (XmlNode _child in _xmlNode.ChildNodes)
                {
                    Nodes[_child.LocalName] = new ConfigNode(_child);
                }

                foreach (XmlAttribute _attrib in _xmlNode.Attributes)
                {
                    Items[_attrib.LocalName] = _attrib.Value;
                }
            }

            public override string ToString()
            {
                StringBuilder _builder = new StringBuilder();
                using (XmlWriter _writer = XmlWriter.Create(_builder))
                {
                    foreach (var _item in Items)
                        _writer.WriteAttributeString(_item.Key, _item.Value);

                    foreach (var _nodes in Nodes)
                    {
                        _writer.WriteStartElement(_nodes.Key);
                        _nodes.Value.Write(_writer);
                        _writer.WriteEndElement();
                    }
                }

                return _builder.ToString();
            }

            public void Write(XmlWriter _writer)
            {
                foreach (var _item in Items)
                    _writer.WriteAttributeString(_item.Key, _item.Value);

                foreach (var _nodes in Nodes)
                {
                    _writer.WriteStartElement(_nodes.Key);
                    _nodes.Value.Write(_writer);
                    _writer.WriteEndElement();
                }
            }
        }

        public override string ToString()
        {
            StringBuilder _builder = new StringBuilder();
            using (MemoryStream _stream = new MemoryStream())
            {
                using (XmlWriter _writer = XmlWriter.Create(_stream))
                {
                    Write(_writer);
                }
                return Encoding.UTF8.GetString(_stream.ToArray());
            }
        }

        public void Write(XmlWriter _writer)
        {
            _writer.WriteStartElement("config");
            foreach (var _obj in Nodes)
            {
                _writer.WriteStartElement(_obj.Key);
                _obj.Value.Write(_writer);
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();
        }

        public void Write(string _filepath)
        {
            using (var w = XmlWriter.Create(File.OpenWrite(_filepath)))
            {
                Write(w);
                w.Flush();
            }
        }

        public static ConfigFile Load(string _filepath)
        {
            Dictionary<string, string[]> _curNode = new Dictionary<string, string[]>();
            Dictionary<string, ConfigNode> _node = new Dictionary<string, ConfigNode>();

            XmlDocument _document = new XmlDocument();
            _document.Load(new StreamReader(File.OpenRead(_filepath), Encoding.UTF8));

            XmlNode _rootNode = _document.ChildNodes[1];
            foreach (XmlNode _child in _rootNode)
            {
                _node[_child.LocalName] = new ConfigNode(_child);
            }

            return new ConfigFile(_node);
        }
    }
}
