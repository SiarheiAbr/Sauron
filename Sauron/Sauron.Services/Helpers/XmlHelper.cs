using System.Xml;

namespace Sauron.Services.Helpers
{
	public static class XmlHelper
	{
		public static XmlNode FindNode(XmlNodeList list, string name, bool findByAttributeName = false)
		{
			if (list.Count > 0)
			{
				foreach (XmlNode node in list)
				{
					if (node.Name.Equals(name) && !findByAttributeName)
					{
						return node;
					}

					if (node.Attributes != null && findByAttributeName)
					{
						foreach (var nodeAttribute in node.Attributes)
						{
							var xmlAttribute = (XmlAttribute)nodeAttribute;

							if (xmlAttribute.Name == "name" && xmlAttribute.InnerText == name)
							{
								return node;
							}
						}
					}

					if (node.HasChildNodes)
					{
						XmlNode nodeFound = FindNode(node.ChildNodes, name, findByAttributeName);
						if (nodeFound != null)
						{
							return nodeFound;
						}
					}
				}
			}

			return null;
		}
	}
}
