using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knife.HologramEffect
{
    [ExecuteAlways]
    public class MatrixProvider : MonoBehaviour
    {
        [SerializeField] private string propertyName = "_CustomMatrix";
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private bool eliminateRootBoneMatrix;

        private MaterialPropertyBlock materialPropertyBlock;

        private void OnEnable()
        {
            if (this == null)
                return;

            if (materialPropertyBlock == null)
                materialPropertyBlock = new MaterialPropertyBlock();

            if (targetRenderers != null)
            {
                foreach (var r in targetRenderers)
                {
                    if (r != null)
                    {
                        Matrix4x4 matrix = transform.localToWorldMatrix;

                        if(eliminateRootBoneMatrix)
                        {
                            SkinnedMeshRenderer skinnedMeshRenderer = r as SkinnedMeshRenderer;

                            if(skinnedMeshRenderer != null)
                            {
                                Transform rootBone = skinnedMeshRenderer.rootBone;

                                if(rootBone != null)
                                {
                                    matrix = rootBone.localToWorldMatrix * matrix;
                                }
                            }
                        }

                        r.GetPropertyBlock(materialPropertyBlock);
                        materialPropertyBlock.SetMatrix(propertyName, matrix);
                        r.SetPropertyBlock(materialPropertyBlock);
                    }
                }
            }
        }

        private void OnValidate()
        {
            OnEnable();
        }
    }
}