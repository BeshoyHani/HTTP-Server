using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace HTTPServer
{
    class XMLWriter
    {
        string []data;
        XmlDocument doc;
        public XMLWriter(string []data)
        {
            this.data = data;
            doc = new XmlDocument();
        }

        public void Save()
        {
            doc.LoadXml("<ClientData></ClientData>");
            for (int i=0; i<data.Length; i+=2)
            {
                XmlElement newElem = doc.CreateElement(data[i]);
                newElem.InnerText = data[i+1];
                doc.DocumentElement.AppendChild(newElem);

            }
            doc.Save("data.xml");
        }
    }
}
