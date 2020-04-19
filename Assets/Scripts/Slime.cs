using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour {

	public GameObject nodePrefab;
	public Rigidbody2D controlNode;
	public Transform face;
    public Transform target;
    public List<string> eaten;

    private Vector3[] vertices, shineVertices;
	private Vector2[] vertices2d;
	public MeshFilter meshFilter, shineMeshFilter;
	private Mesh mesh, shineMesh;
	public LineRenderer outline;
    public float speed = 1f;

	private Transform[] nodes;
	private int[] indices;
    private float size = 0.5f;

	private int points = 50;

	private float faceAngle = 0f;
	private float shineDistance = 0.75f;

	private bool sleeping = false;

	public float radius = 3.5f;

	public int slimeColor = 1;

	public PolygonCollider2D polyCollider;

	public bool inside = false;

	// Use this for initialization
	void Start () {

        eaten = new List<string>();

        //Resize();

        RandomizeSpawn();

        mesh = meshFilter.mesh;
		shineMesh = shineMeshFilter.mesh;

		vertices = new Vector3[points];
		vertices2d = new Vector2[points];
		shineVertices = new Vector3[points];
		nodes = new Transform[points];

        outline.positionCount = points;

		Rigidbody2D first = null;
		Rigidbody2D prev = null;

		for (int i = 0; i < points; i++) {

			float angle = i / (float)points * Mathf.PI * 2;
			GameObject node = Instantiate (nodePrefab, transform);
			node.transform.localPosition = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0) * radius;

			node.transform.Rotate(new Vector3(0, 0, -90 + angle / Mathf.PI * 180));

			if (prev) {
                var joint = node.GetComponent<HingeJoint2D>();
                joint.connectedBody = prev;
                joint.connectedAnchor = new Vector2 (-0.5f, 0);
			}

			prev = node.GetComponent<Rigidbody2D> ();

			if (!first) {
				first = prev;
			}

			vertices [i] = node.transform.localPosition + node.transform.rotation * (Vector3.right * 0.5f);
			shineVertices [i] = Vector3.MoveTowards(node.transform.localPosition, controlNode.transform.localPosition, shineDistance);
			vertices [i].z = 0.01f;
			shineVertices [i].z = 0;
			nodes [i] = node.transform;

			vertices2d [i] = (Vector2)vertices [i];
		}

		first.GetComponent<HingeJoint2D> ().connectedBody = prev;

		UpdateMesh ();

        AudioManager.Instance.PlayEffectAt(0, transform.position, 1.782f);
        AudioManager.Instance.PlayEffectAt(5, transform.position, 1.36f);
        AudioManager.Instance.PlayEffectAt(8, transform.position, 1.166f);
        AudioManager.Instance.PlayEffectAt(11, transform.position, 1.255f);
        AudioManager.Instance.PlayEffectAt(19, transform.position, 1.004f);

    }

    private void UpdateVerts() {
		for (int i = 0; i < points; i++) {
			vertices [i] = nodes [i].localPosition + nodes [i].rotation * (Vector3.right * 0.5f);
			shineVertices [i] = Vector3.MoveTowards(nodes [i].transform.localPosition, controlNode.transform.localPosition, shineDistance);
			vertices [i].z = 0.1f;
			shineVertices [i].z = 0;
			vertices2d [i] = Vector2.MoveTowards((Vector2)vertices [i], controlNode.transform.localPosition, 0.3f);
		}
	}

	private void UpdateMesh() {

        // Use the triangulator to get indices for creating triangles
        SlimeTriangulator tr = new SlimeTriangulator(vertices);
		indices = tr.Triangulate();

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		// Use the triangulator to get indices for creating triangles
		tr = new SlimeTriangulator(shineVertices);
		int[] shineIndices = tr.Triangulate();

		shineMesh.vertices = shineVertices;
		shineMesh.triangles = shineIndices;
		shineMesh.RecalculateNormals();
		shineMesh.RecalculateBounds();

		outline.SetPositions (vertices);

		polyCollider.SetPath (0, vertices2d);
	}

	private void PositionFace() {

		if (Random.value < 0.01f) {
			faceAngle = Random.Range (-20f, 20f);
		}

		float diff = (face.transform.localPosition - controlNode.transform.localPosition).magnitude;

		face.transform.localPosition = Vector3.MoveTowards (face.transform.localPosition, controlNode.transform.localPosition, diff * 0.2f);

		if (!sleeping) {
			face.transform.rotation = Quaternion.RotateTowards (face.transform.rotation, Quaternion.Euler (new Vector3 (0, 0, faceAngle)), 0.3f);
			face.transform.localScale = Vector3.one - Mathf.Abs (Mathf.Sin (Time.time * 3f)) * Vector3.one * 0.05f;
			face.transform.localPosition += Mathf.Abs(Mathf.Sin (Time.time * 5f)) * Vector3.up * 0.05f;
		}

		face.transform.position = new Vector3(face.transform.position.x, face.transform.position.y, -2.1f);
	}

	private void PositionShine() {
		Vector3 lightPoint = Vector3.zero;
		Vector3 direction = (lightPoint - controlNode.transform.position).normalized;
		direction = direction * 0.4f;
		direction.z = shineMeshFilter.transform.localPosition.z;
//		shineMeshFilter.transform.localPosition = Vector3.MoveTowards(shineMeshFilter.transform.localPosition, direction, 0.1f);
	}

	void Update() {

        //size = Mathf.MoveTowards(size, 5f, 0.001f);
        //Resize();

        UpdateVerts ();
		UpdateMesh ();

		PositionShine ();

		PositionFace ();

		if (target) {
			Vector2 dir = (Vector2)controlNode.transform.position - (Vector2)target.position;
			controlNode.AddForce (-dir * 2f * Time.deltaTime * 60f * speed, ForceMode2D.Force);
		}
	}

    private void LateUpdate()
    {
        
    }

    void Resize()
    {
        transform.localScale = Vector3.one * size;
    }

    public void RandomizeSpawn()
    {
        var dir = Random.value < 0.5f ? 1f : -1f;
        transform.position = new Vector3(dir * 25f, 0f, -2f);
    }
}
