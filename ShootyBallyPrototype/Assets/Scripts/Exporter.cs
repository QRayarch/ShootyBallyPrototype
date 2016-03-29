using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

[ExecuteInEditMode]
public class Exporter : MonoBehaviour {
#if UNITY_EDITOR
    private const string FILE_EXTENTION = "txt";
    private const string IMAGE_FILE_EXTENTION = "png";//Note changing this does not actually change the file encoding
    private const string K_MAP_NAME = "name";
    private const string K_MAP_IMAGE = "image";
    private const string K_MAP_BOUNDS = "bounds";
    private const string K_POS = "pos";
    private const string K_ROT = "rot";
    private const string K_SCALE = "scl";
    private const string K_COLOR = "color";
    private const string K_MODEL = "model";
    private const string K_MATERIAL = "material";
    private const string K_TEXTURE = "texture";
    private const string K_TURRETS = "turrets";
    private const string K_PLAYER_NUMBER = "playerNum";
    private const string K_BALL = "ball";
    private const string K_LIGHTING = "lights";
    private const string K_LIGHTING_TYPE = "lType";
    private const string K_LIGHT_INTENSITY = "lIntensity";
    private const string K_BACKGROUND = "background";
    private const string K_ARENA = "arena";

    public string mapName = "Untitled";
    public int imageWidth = 800;
    public int imageHeight = 600;
    public Vector3 exportRotation;
    public bool generateImage = true;

    public bool export = false;

    private Quaternion rotation;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if(export)
        {
            Export();
        }
	}

    public void Export()
    {
        rotation = Quaternion.Euler(exportRotation);
        export = false;
        string path = EditorUtility.SaveFilePanel(mapName, "", mapName + "." + FILE_EXTENTION, FILE_EXTENTION);
        if(path.Length != 0)
        {
            StreamWriter sw = File.CreateText(path);
            GenerateMapPicture(path);
            GenerateMapFile(sw);
            sw.Close();
        }
    }

    private void GenerateMapPicture(string path)
    {
        //Disable all Lighting
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        for (int l = 0; l < lights.Length; l++)
        {
            lights[l].enabled = false;
        }

        GameObject cameraGameObject = new GameObject();
        Camera mapCamera = cameraGameObject.AddComponent<Camera>();
        mapCamera.orthographic = true;
        //Adjust Camera
        GoalSystem goalSys = GameObject.FindObjectOfType<GoalSystem>();
        if (!goalSys)
        {
            Debug.LogError("--Export Image Warning--//The image export couldn't find a goal system so the image isn't centered accuratly.");
            return;
        }
        float width = goalSys.arenaMax.x + goalSys.arenaMin.x;
        float height = goalSys.arenaMax.y + goalSys.arenaMin.y;
        mapCamera.transform.position = new Vector2(width, height) * 0.5f + (Vector2)goalSys.transform.position - goalSys.arenaMin;
        mapCamera.orthographicSize = Mathf.Max(height / 2.0f, (width / (imageWidth / (float)imageHeight)) / 2.0f);
        //Continue the image saving
        RenderTexture texture = new RenderTexture(imageWidth, imageHeight, 24);
        mapCamera.targetTexture = texture;
        Texture2D image = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);
        mapCamera.Render();
        RenderTexture.active = texture;
        image.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        RenderTexture.active = null;
        byte[] imageData = image.EncodeToPNG();
        string imagePath = path.Substring(0, path.LastIndexOf('.')) + "." + IMAGE_FILE_EXTENTION;
        File.WriteAllBytes(imagePath, imageData);
        mapCamera.targetTexture = null;
        DestroyImmediate(cameraGameObject);
        DestroyImmediate(texture);

        for (int l = 0; l < lights.Length; l++)
        {
            lights[l].enabled = true;
        }
    }

    public void GenerateMapFile(StreamWriter sw)
    {
        //Write the top level stuff
        sw.WriteLine(K_MAP_NAME + " " + mapName);
        sw.WriteLine(K_MAP_IMAGE + " " + mapName + "." + IMAGE_FILE_EXTENTION);

        //Write the player turrets
        GoalSystem goalSys = GameObject.FindObjectOfType<GoalSystem>();
        if(!goalSys)
        {
            Debug.LogError("--Export Failure--//The map export failed because no Goal System was found in the scene.");
            return;
        }
        Write(sw, K_TURRETS, goalSys.turretStartZAngle);
        PlayerTurret[] turrets = GameObject.FindObjectsOfType<PlayerTurret>();
        if(turrets.Length == 0)
        {
            Debug.LogWarning("--Export Warrning--//The map exported, but there are no player turrets in the scene.");
        }
        for(int i = 0; i < turrets.Length; i++)
        {
            Write(sw, K_PLAYER_NUMBER, ((int)turrets[i].player) - 1);
            Write(sw, turrets[i].transform, true, false, false);
        }

        //Write the ball
        Write(sw, K_BALL);
        Write(sw, K_POS, new Vector3(goalSys.ballStartPos.x, goalSys.ballStartPos.y, goalSys.ball.position.z));

        //Write Lighting
        Write(sw, K_LIGHTING);
        Light[] lights = GameObject.FindObjectsOfType<Light>();
        for(int l = 0; l < lights.Length; l++)
        {
            if(lights[l].CompareTag("Export_Light"))
            {
                Write(sw, K_LIGHTING_TYPE, ((int)lights[l].type) - 1);
                Write(sw, lights[l].transform, true, true, false);
                Write(sw, K_COLOR, lights[l].color);
                Write(sw, K_LIGHT_INTENSITY, lights[l].intensity);
            }
        }

        //Write the Background
        Write(sw, K_BACKGROUND);
        GameObject[] backgroundPieces = GameObject.FindGameObjectsWithTag("Export_Background");
        for(int b = 0; b < backgroundPieces.Length; b++)
        {
            MeshFilter filter = backgroundPieces[b].GetComponent<MeshFilter>();
            if(filter != null)
            {
                Write(sw, filter.sharedMesh);
                MeshRenderer render = backgroundPieces[b].GetComponent<MeshRenderer>();
                if(render != null)
                {
                    Write(sw, render.sharedMaterial);
                }
                Write(sw, backgroundPieces[b].transform);
            }
        }

        //Write the Arena
        Write(sw, K_ARENA);
        sw.WriteLine(K_MAP_BOUNDS + " " + goalSys.arenaMin.x + " " + goalSys.arenaMin.y + " " + goalSys.arenaMax.x + " " + goalSys.arenaMax.y);
        GameObject[] arenaPieces = GameObject.FindGameObjectsWithTag("Export_Arena");
        for (int a = 0; a < arenaPieces.Length; a++)
        {
            MeshFilter filter = arenaPieces[a].GetComponent<MeshFilter>();
            if (filter != null)
            {
                Write(sw, filter.sharedMesh);
                MeshRenderer render = arenaPieces[a].GetComponent<MeshRenderer>();
                if (render != null)
                {
                    Write(sw, render.sharedMaterial);
                }
                Write(sw, arenaPieces[a].transform);

            }
        }
    }

    private void Write(StreamWriter sw, string prefix)
    {
        sw.WriteLine(prefix);
    }

    private void Write(StreamWriter sw, string prefix, float val)
    {
        sw.WriteLine(prefix + " " + val.ToString());
    }

    private void Write(StreamWriter sw, string prefix, int val)
    {
        sw.WriteLine(prefix + " " + val.ToString());
    }

    private void Write(StreamWriter sw, string prefix, Color color)
    {
        sw.WriteLine(prefix + " " + color.r + " " + color.g + " " + color.b + " " + color.a);
    }

    private void Write(StreamWriter sw, string prefix, Vector3 vector)
    {
        sw.WriteLine(prefix + " " + vector.x + " " + vector.y + " " + vector.z);
    }

    private void Write(StreamWriter sw, string prefix, Quaternion rot)
    {
        Write(sw, prefix, Mathf.Deg2Rad * (rot.eulerAngles));
    }

    private void Write(StreamWriter sw, Transform t, bool pos = true, bool rot = true, bool scl = true)
    {
        if(pos) Write(sw, K_POS, rotation * t.position);
        if (rot) Write(sw, K_ROT, rotation * t.rotation);
        if (scl) Write(sw, K_SCALE, t.lossyScale);
    }

    private void Write(StreamWriter sw, Mesh m)
    {
        sw.WriteLine(K_MODEL + " " + RemoveAfterSpace(m.name));
    }

    private void Write(StreamWriter sw, Material mat, bool onlyTextures = false, bool textures = true)
    {
        if(!onlyTextures)
        {
            sw.WriteLine(K_MATERIAL + " " + RemoveAfterSpace(mat.shader.name));
        }
        if(textures || onlyTextures)
        {
            List<Texture> textureList = new List<Texture>();
            for(int p = 0; p < ShaderUtil.GetPropertyCount(mat.shader); p++)
            {
                if(ShaderUtil.GetPropertyType(mat.shader, p) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    Texture t = mat.GetTexture(ShaderUtil.GetPropertyName(mat.shader, p));
                    if(t != null)
                    {
                        textureList.Add(t);
                    }
                }
            }
            Write(sw, textureList);
        }
    }

    private void Write(StreamWriter sw, Texture text)
    {
        if (text == null) return;
        sw.WriteLine(K_TEXTURE + " " + RemoveAfterSpace(text.name));
    }

    private void Write(StreamWriter sw, List<Texture> textures)
    {
        string texturePaths = "";
        for(int t = 0; t < textures.Count; t++)
        {
            texturePaths += RemoveAfterSpace(textures[t].name) + " ";
        }
        sw.WriteLine(K_TEXTURE + " " + texturePaths);
    }

    private string RemoveAfterSpace(string s)
    {
        if (s.IndexOf(' ') == -1) return s;
        return s.Substring(0, s.IndexOf(' '));
    }
#endif
}
