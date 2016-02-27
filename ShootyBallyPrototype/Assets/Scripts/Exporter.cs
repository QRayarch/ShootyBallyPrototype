using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class Exporter : MonoBehaviour {

    private const string FILE_EXTENTION = "txt";
    private const string K_MAP_NAME = "name";
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
    private const string K_BACKGROUND = "background";
    private const string K_ARENA = "arena";

    public string mapName = "Untitled";

    public bool export = false;

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
        export = false;
        string path = EditorUtility.SaveFilePanel(mapName, "", mapName + "." + FILE_EXTENTION, FILE_EXTENTION);
        if(path.Length != 0)
        {
            StreamWriter sw = File.CreateText(path);
            GenerateMapFile(sw);
            sw.Close();
        }
    }

    public void GenerateMapFile(StreamWriter sw)
    {
        //Write the top level stuff
        sw.WriteLine(K_MAP_NAME + " " + mapName);

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
        GameObject[] arenaPieces = GameObject.FindGameObjectsWithTag("Export_Arena");
        for (int a = 0; a < backgroundPieces.Length; a++)
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
        Write(sw, prefix, rot.eulerAngles);
    }

    private void Write(StreamWriter sw, Transform t, bool pos = true, bool rot = true, bool scl = true)
    {
        if(pos) Write(sw, K_POS, t.position);
        if (rot) Write(sw, K_ROT, t.rotation);
        if (scl) Write(sw, K_SCALE, t.localScale);
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
            Write(sw, mat.mainTexture);
        }
    }

    private void Write(StreamWriter sw, Texture text)
    {
        if (text == null) return;
        sw.WriteLine(K_TEXTURE + " " + RemoveAfterSpace(text.name));
    }

    private string RemoveAfterSpace(string s)
    {
        if (s.IndexOf(' ') == -1) return s;
        return s.Substring(0, s.IndexOf(' '));
    }
}
