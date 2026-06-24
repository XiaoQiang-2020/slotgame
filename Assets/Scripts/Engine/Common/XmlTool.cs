/***************************************************
 * 文件名： XmlTool.cs
 * 描  述： Xml文件工具（只具备读取功能,不具备写入更改内容）
 * 时  间： 2017-11-29 09:52:29
 * 作  者： 李小冬
 * 修  改： 用来读取Xml文件
 ***************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using Core;

namespace Game
{
    public class XmlTool : Core.Singleton<XmlTool>
    {
        /// <summary>
        /// 提取XML节点下指定节点的value值
        /// </summary>
        /// <param name="rootNode">节点</param>
        /// <param name="xPath">指定节点路径</param>
        /// <returns>XML根节点下指定节点的value值</returns>
        public string GetXMLNodeValue(XmlNode rootNode, string xPath)
        {
            XmlNode node = GetXMLNode(rootNode, xPath);
            return GetXMLNodeValue(node);
        }

        /// <summary>
        /// 提取XML节点下指定节点的属性值
        /// </summary>
        /// <param name="rootNode">节点</param>
        /// <param name="xPath">指定节点路径</param>
        /// <param name="attrName">属性名</param>
        /// <returns>XML节点下指定节点的属性值</returns>
        public string GetXMLNodeAttribute(XmlNode rootNode, string xPath, string attrName)
        {
            XmlNode node = GetXMLNode(rootNode, xPath);
            return GetXMLAttributeValue(node, attrName);
        }


        /// <summary>
        /// 提取XML文件
        /// </summary>
        /// <param name="xmlString">字符串XML文件信息</param>
        /// <returns>XML实例</returns>
        public XmlDocument LoadXMLFile(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                Debuger.LogError("加载XML文件时,所传参数为空,异常");
                return null;
            }

            //XML实例
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            return xmlDoc;
        }

        /// <summary>
        /// 获取XML文件根节点
        /// </summary>
        /// <param name="xmlDoc">XML实例</param>
        /// <returns>XML文件根节点</returns>
        public XmlNode GetXMLRootNode(XmlDocument xmlDoc)
        {
            if (null == xmlDoc)
            {
                Debuger.LogError("获取XML文件根节点时,所传参数为空,异常");
                return null;
            }

            XmlNode rootNode = xmlDoc.FirstChild;
            if (null != rootNode)
            {
                return rootNode;
            }
            else
            {
                Debuger.LogWarning("获取XML文件根节点时,为空,异常");
                return null;
            }
        }

        /// <summary>
        /// 获取XML文件根节点名
        /// </summary>
        /// <param name="xmlDoc">XML实例</param>
        /// <returns>XML文件根节点名</returns>
        public string GetXMLRootNodeName(XmlDocument xmlDoc)
        {
            if (null == xmlDoc)
            {
                Debuger.LogError("获取XML文件根节点名时,所传参数为空,异常");
                return null;
            }

            XmlNode rootNode = GetXMLRootNode(xmlDoc);

            if (null != rootNode)
            {
                return rootNode.Name;
            }
            else
            {
                Debuger.LogWarning("获取XML文件根节点时,为空,异常");
                return null;
            }
        }

        /// <summary>
        /// 获取XML指定节点下特定节点
        /// </summary>
        /// <param name="xmlNode">XML指定节点</param>
        /// <param name="xPath">XML特定节点名</param>
        /// <returns>XML特定节点</returns>
        public XmlNode GetXMLNode(XmlNode xmlNode, string xPath)
        {
            if (null == xmlNode || string.IsNullOrEmpty(xPath))
            {
                Debuger.LogError("获取XML指定节点下特定节点时,传入参数异常");
                return null;
            }

            try
            {
                if (xmlNode.HasChildNodes)
                {
                    XmlNode node = xmlNode.SelectSingleNode(xPath);
                    if (null != node)
                        return node;
                    else
                    {
                        Debuger.LogWarning("获取XML指定节点下特定节点时,指定节点下并未存在特定节点：" + xPath);
                        return null;
                    }
                }
                else
                {
                    Debuger.LogWarning("获取XML指定节点下特定节点时,指定节点下并未存在任何子节点,异常");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debuger.LogError(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取XML指定节点特定属性值
        /// </summary>
        /// <param name="xmlNode">指定节点</param>
        /// <param name="attriName">属性名</param>
        /// <returns>属性值</returns>
        public string GetXMLAttributeValue(XmlNode xmlNode, string attriName)
        {
            if (null == xmlNode || string.IsNullOrEmpty(attriName))
            {
                Debuger.LogError("获取XML指定节点时,所传参数为空,异常");
                return null;
            }

            try
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    XmlAttribute attri = xmlNode.Attributes[attriName];
                    if (null != attri)
                        return attri.Value;
                    else
                    {
                        Debuger.LogError("获取XML指定节点时,获取属性为空,异常！属性：" + attriName);
                        return null;
                    }
                }
                else
                {
                    Debuger.LogError("获取XML指定节点时,节点无属性,异常");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debuger.LogError(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取XML指定节点Value值
        /// </summary>
        /// <param name="xmlNode">指定节点</param>
        /// <returns>XML指定节点Value值</returns>
        public string GetXMLNodeValue(XmlNode xmlNode)
        {
            if (null == xmlNode)
            {
                Debuger.LogError("获取XML指定节点Value值时,所传参数为空,异常");
                return null;
            }

            try
            {
                return xmlNode.InnerText;
            }
            catch (Exception ex)
            {
                Debuger.LogError(ex.ToString());
                return null;
            }
        }
    }
}