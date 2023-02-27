using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTrackableObject : MonoBehaviour
{
    protected Texture MainTexture;
    public Texture MainTextureProperty
    {
        get { return MainTexture; }
    }
}
