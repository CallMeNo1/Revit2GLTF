﻿using Autodesk.Revit.DB.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit2Gltf.glTF
{
    public class gltfUtil
    {
        public static void addVec3BufferViewAndAccessor(GLTF gltf, glTFBinaryData bufferData)
        {
            var v3ds = bufferData.vertexBuffer;
            var byteOffset = 0;
            if (gltf.bufferViews.Count > 0)
            {
                byteOffset = gltf.bufferViews[gltf.bufferViews.Count - 1].byteLength + gltf.bufferViews[gltf.bufferViews.Count - 1].byteOffset;
            }
            var bufferIndex = 0;
            var vec3View = gltfUtil.addBufferView(bufferIndex, byteOffset, 4 * v3ds.Count);
            vec3View.target = Targets.ARRAY_BUFFER;
            gltf.bufferViews.Add(vec3View);
            var vecAccessor = gltfUtil.addAccessor(gltf.bufferViews.Count - 1, 0, ComponentType.FLOAT, v3ds.Count / 3, AccessorType.VEC3);
            var minAndMax = gltfUtil.GetVec3MinMax(v3ds);
            vecAccessor.min = new List<double>() { minAndMax[0], minAndMax[1], minAndMax[2] };
            vecAccessor.max = new List<double>() { minAndMax[3], minAndMax[4], minAndMax[5] };
            gltf.accessors.Add(vecAccessor);
        }

        public static void addNormalBufferViewAndAccessor(GLTF gltf, glTFBinaryData bufferData)
        {
            var v3ds = bufferData.normalBuffer;
            var byteOffset = 0;
            if (gltf.bufferViews.Count > 0)
            {
                byteOffset = gltf.bufferViews[gltf.bufferViews.Count - 1].byteLength + gltf.bufferViews[gltf.bufferViews.Count - 1].byteOffset;
            }
            var bufferIndex = 0;
            var vec3View = gltfUtil.addBufferView(bufferIndex, byteOffset, 4 * v3ds.Count);
            vec3View.target = Targets.ARRAY_BUFFER;
            gltf.bufferViews.Add(vec3View);
            var vecAccessor = gltfUtil.addAccessor(gltf.bufferViews.Count - 1, 0, ComponentType.FLOAT, v3ds.Count / 3, AccessorType.VEC3);
            gltf.accessors.Add(vecAccessor);
        }


        public static void addIndexsBufferViewAndAccessor(GLTF gltf,glTFBinaryData bufferData)
        {
            var byteOffset = 0;
            if (gltf.bufferViews.Count > 0)
            {
                byteOffset = gltf.bufferViews[gltf.bufferViews.Count - 1].byteLength + gltf.bufferViews[gltf.bufferViews.Count - 1].byteOffset;
            }
            var bufferIndex = 0;
            glTFBufferView faceView;
            glTFAccessor faceAccessor;
            var length = bufferData.indexBuffer.Count;
            if (bufferData.indexMax > 65535)
            {
                faceView = gltfUtil.addBufferView(bufferIndex, byteOffset, 4 * length);
                faceView.target = Targets.ELEMENT_ARRAY_BUFFER;
                gltf.bufferViews.Add(faceView);
                faceAccessor = gltfUtil.addAccessor(gltf.bufferViews.Count - 1, 0, ComponentType.UNSIGNED_INT, length, AccessorType.SCALAR);
            }
            else
            {
                var align = 0;
                if (2 * length % 4 != 0)
                {
                    align = 2;
                    bufferData.indexAlign = 0x20;
                }
                faceView = gltfUtil.addBufferView(bufferIndex, byteOffset, 2 * length + align);
                faceView.target = Targets.ELEMENT_ARRAY_BUFFER;
                gltf.bufferViews.Add(faceView);
                faceAccessor = gltfUtil.addAccessor(gltf.bufferViews.Count - 1, 0, ComponentType.UNSIGNED_SHORT, length, AccessorType.SCALAR);
            }
            gltf.accessors.Add(faceAccessor);
        }

        public static void addUvBufferViewAndAccessor(GLTF gltf, glTFBinaryData bufferData)
        {
            var uvs = bufferData.uvBuffer;
            var byteOffset = 0;
            if (gltf.bufferViews.Count > 0)
            {
                byteOffset = gltf.bufferViews[gltf.bufferViews.Count - 1].byteLength + gltf.bufferViews[gltf.bufferViews.Count - 1].byteOffset;
            }
            var bufferIndex = 0;
            var vec3View = gltfUtil.addBufferView(bufferIndex, byteOffset, 4 * uvs.Count);
            vec3View.target = Targets.ARRAY_BUFFER;
            gltf.bufferViews.Add(vec3View);
            var vecAccessor = gltfUtil.addAccessor(gltf.bufferViews.Count - 1, 0, ComponentType.FLOAT, uvs.Count / 2, AccessorType.VEC2);
            gltf.accessors.Add(vecAccessor);
        }

        public static glTFBufferView addBufferView(int bufferIndex, int byteOffset, int byteLength)
        {
            var bufferView = new glTFBufferView();
            bufferView.buffer = bufferIndex;
            bufferView.byteOffset = byteOffset;
            bufferView.byteLength = byteLength;
            return bufferView;
        }

        public static glTFAccessor addAccessor(int bufferView, int byteOffset, ComponentType componentType,int count, string type)
        {
            var accessor = new glTFAccessor();
            accessor.bufferView = bufferView;
            accessor.byteOffset = byteOffset;
            accessor.componentType = componentType;
            accessor.count = count;
            accessor.type = type;
            return accessor;
        }


        public static double[] GetVec3MinMax(List<double> vec3)
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            List<double> zValues = new List<double>();
            for (int i = 0; i < vec3.Count; i++)
            {
                if ((i % 3) == 0) xValues.Add(vec3[i]);
                if ((i % 3) == 1) yValues.Add(vec3[i]);
                if ((i % 3) == 2) zValues.Add(vec3[i]);
            }
            double maxX = xValues.Max();
            double minX = xValues.Min();
            double maxY = yValues.Max();
            double minY = yValues.Min();
            double maxZ = zValues.Max();
            double minZ = zValues.Min();
            return new double[] { minX, minY, minZ, maxX, maxY, maxZ };
        }



        public static string ReadAssetProperty(AssetProperty prop)
        {
            switch (prop.Type)
            {
#if  Revit2016 || Revit2017
                case AssetPropertyType.APT_String:
#else
                case AssetPropertyType.String:
#endif
                    AssetPropertyString val = prop as AssetPropertyString;
                    if (val.Name == "unifiedbitmap_Bitmap")
                    {
                        return val.Value;
                    }
                    break;
                // The APT_List contains a list of sub asset properties with same type.
#if  Revit2016 || Revit2017
                case AssetPropertyType.APT_List:
#else
                case AssetPropertyType.List:
#endif
                    AssetPropertyList propList = prop as AssetPropertyList;
                    IList<AssetProperty> subProps = propList.GetValue();
                    if (subProps.Count == 0)
                        break;
                    switch (subProps[0].Type)
                    {
#if Revit2016 || Revit2017
               case AssetPropertyType.APT_Integer:
#else
                        case AssetPropertyType.Integer:
#endif
                            AssetPropertyString val2 = prop as AssetPropertyString;
                            if (val2.Name == "unifiedbitmap_Bitmap")
                            {
                                return val2.Value;
                            }
                            break;
                    }
                    break;
#if Revit2016 || Revit2017
                case AssetPropertyType.APT_Asset:
#else
                case AssetPropertyType.Asset:
#endif
                    Asset propAsset = prop as Asset;
                    var value = ReadAsset(propAsset);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                    break;
            }
            if (prop.NumberOfConnectedProperties == 0)
                return null;

            if (prop.Name == "generic_diffuse")
            {
                foreach (AssetProperty connectedProp in prop.GetAllConnectedProperties())
                {
                    var value = ReadAssetProperty(connectedProp);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }
            return null;
        }
        public static string ReadAsset(Asset asset)
        {
            for (int idx = 0; idx < asset.Size; idx++)
            {
                AssetProperty prop = asset[idx];
                var value = ReadAssetProperty(prop);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            return null ;
        }



    }
}
