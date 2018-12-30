using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generator for holo terrain mesh and height map (bases on compute shader TerrainGeneratorShader)
/// </summary>
public class HoloTerrainGenerator : MonoBehaviour
{
	[SerializeField] private ComputeShader terrainGeneratorShader;
	[SerializeField] private bool createCopyOfTerrainMaterial;
	[SerializeField] private Material terrainRendererMaterial;
	[SerializeField] private int xSize, ySize;
	[SerializeField] private float density;
	[Tooltip("Under this value of height map terrain will be flatened")]
	[SerializeField] private float bottomTerrainClamping = 0;
	[Tooltip("Under this value of height map terrain will be stretched simmilary to widening the contrast in images")]
	[SerializeField] private float bottomTerrainStretch = 0;
	[Tooltip("Over this value of height map terrain will be stretched simmilary to widening the contrast in images")]
	[SerializeField] private float topTerrainStretch = 1;
	[SerializeField] private Vector2 terrainRendererSize;
	[Tooltip("How frequently (per quad in mesh) red channel of vertices will be ping-ponged")]
	[SerializeField]private int quadCounterSplitRatio = 64;

	private int computeKernelId;
	private RenderTexture heightMapTex;
	private float[] heightMap;
	private ComputeBuffer heightMapBuffer;
	private MeshFilter meshFilter;
	private Material meshMaterial;

	#region UnityMethods
	protected void Awake()
	{
		Trans = transform;
		Initialize();
	}
	
	/*protected void Update()
	{
		Refresh(Time.time, Time.time);
	}*/

	protected void OnDestroy()
	{
		if(heightMapBuffer != null)
			heightMapBuffer.Release();
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position, new Vector3(terrainRendererSize.x, 0.1f, terrainRendererSize.y));
	}
	#endregion

	public void Initialize()
	{
		InitializeComputations();
		InitializeRenderer();
	}

	public void Refresh(float xOffset, float yOffset)
	{
		terrainGeneratorShader.SetFloat("xOffset", xOffset);
		terrainGeneratorShader.SetFloat("yOffset", yOffset);
		terrainGeneratorShader.SetFloat("density", density);
		terrainGeneratorShader.SetFloat("bottomClamping", bottomTerrainClamping);
		terrainGeneratorShader.SetFloat("bottomStretch", bottomTerrainStretch);
		terrainGeneratorShader.SetFloat("topStretch", topTerrainStretch);
		terrainGeneratorShader.Dispatch(computeKernelId, xSize / 32, ySize / 32, 1);
		heightMapBuffer.GetData(heightMap);
	}

	public float GetTerrainHeight(float worldX, float worldZ)
	{
		int xPos = Mathf.CeilToInt((worldX - Trans.position.x + terrainRendererSize.x * 0.5f) / terrainRendererSize.x * xSize);
		int yPos = Mathf.CeilToInt((worldZ - Trans.position.z + terrainRendererSize.y * 0.5f) / terrainRendererSize.y * ySize);
		if(xPos < 0)
		{
			xPos = 0;
		}
		if(xPos >= xSize)
		{
			xPos = xSize - 1;
		}
		if(yPos < 0)
		{
			yPos = 0;
		}
		if(yPos >= ySize)
		{
			yPos = ySize - 1;
		}

		return heightMap[xPos + yPos * xSize];
	}

	public void SetHeightFactorToTerrainMaterial(float heightFactor)
	{
		meshMaterial.SetFloat("_HeightFactor", heightFactor);
		LastHeightFactor = heightFactor;
	}

	private void InitializeComputations()
	{
		computeKernelId = terrainGeneratorShader.FindKernel("CSMain");
		heightMapTex = new RenderTexture(xSize, ySize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		heightMapTex.enableRandomWrite = true;
		heightMapTex.Create();
		terrainGeneratorShader.SetTexture(computeKernelId, "terrainHeightMapTex", heightMapTex);
		heightMap = new float[xSize * ySize];
		heightMapBuffer = new ComputeBuffer(xSize * ySize, sizeof(float));
		heightMapBuffer.SetData(heightMap);
		terrainGeneratorShader.SetBuffer(computeKernelId, "terrainHeightMap", heightMapBuffer);
		terrainGeneratorShader.SetInt("terrainTextureWidth", xSize);
	}

	private void InitializeRenderer()
	{
		if(meshFilter == null)
		{
			GameObject go = new GameObject("TerrainRenderer");
			go.layer = gameObject.layer;
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			meshFilter = go.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
			meshMaterial = createCopyOfTerrainMaterial ? new Material(terrainRendererMaterial) : terrainRendererMaterial;
			meshRenderer.sharedMaterial = meshMaterial;
		}
		
		Mesh mesh = new Mesh();
		mesh.name = "HoloTerrain";
		Vector3[] vertices = new Vector3[xSize * ySize * 4];
		Color[] colors = new Color[vertices.Length];
		int[] indices = new int[xSize * ySize * 6];

		int quadCounter = 0;
		int vertexCounter = 0;
		int indexCounter = 0;
		float xOffset = -terrainRendererSize.x * 0.5f;
		float zOffset = -terrainRendererSize.y * 0.5f;
		float xStep = terrainRendererSize.x / xSize;
		float zStep = terrainRendererSize.y / ySize;
		for(int x = 0; x < xSize; x++)
		{
			for(int z = 0; z < ySize; z++)
			{
				indices[indexCounter++] = vertexCounter;
				indices[indexCounter++] = vertexCounter + 1;
				indices[indexCounter++] = vertexCounter + 2;
				indices[indexCounter++] = vertexCounter;
				indices[indexCounter++] = vertexCounter + 2;
				indices[indexCounter++] = vertexCounter + 3;

				vertices[vertexCounter] = new Vector3(xStep * x + xOffset, 0f, zStep * z + zOffset);
				vertices[vertexCounter + 1] = new Vector3(xStep * x + xOffset, 0f, zStep * (z + 1) + zOffset);
				vertices[vertexCounter + 2] = new Vector3(xStep * (x + 1) + xOffset, 0f, zStep * (z + 1) + zOffset);
				vertices[vertexCounter + 3] = new Vector3(xStep * (x + 1) + xOffset, 0f, zStep * z + zOffset);

				Color color = new Color(Mathf.PingPong(quadCounter / (float)quadCounterSplitRatio, 1f), UnityEngine.Random.value, 0f, 1f);
				for(int i = 0; i < 4; i++)
				{
					colors[vertexCounter++] = color;
				}

				quadCounter++;
			}
		}

		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.triangles = indices;
		mesh.RecalculateBounds();
		meshFilter.sharedMesh = mesh;

		meshMaterial.SetTexture("_HeightMap", heightMapTex);
		meshMaterial.SetVector("_HeightMapParams", new Vector4(Trans.position.x + xOffset,
																Trans.position.z + zOffset,
																terrainRendererSize.x, 
																terrainRendererSize.y));
	}

	public Transform Trans { get; private set; }
	public float BottomTerrainY
	{
		get
		{
			return Trans.position.y;
		}
	}

	public float LastHeightFactor { get; private set; }

	public Material MeshMaterial
	{
		get
		{
			return meshMaterial;
		}
	}
}
