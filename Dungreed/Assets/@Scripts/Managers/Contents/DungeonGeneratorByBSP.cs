using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect; //�и��� ������ rect����
    public RectInt roomRect; //�и��� ���� �� ���� rect����
    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
        //���� ��� ��. ��� ���� ���� �� ���
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}
public class DungeonGeneratorByBSP : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //������ �������� �ּ� ����
    [SerializeField] float maximumDivideRate; //������ �������� �ִ� ����
    /*[SerializeField] private GameObject line; //lineRenderer�� ����ؼ� ������ �������� ���������� �����ֱ� ����
    [SerializeField] private GameObject map; //lineRenderer�� ����ؼ� ù ���� ����� �����ֱ� ����
    [SerializeField] private GameObject roomLine; //lineRenderer�� ����ؼ� ���� ����� �����ֱ� ����*/
    [SerializeField] private int maximumDepth; //Ʈ���� ����, ���� ���� ���� �� �ڼ��� ������ ��
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile[] roomTile; //���� �����ϴ� Ÿ��
    [SerializeField] Tile[] wallTile; //��� �ܺθ� ���������� �� Ÿ��
    [SerializeField] Tile[] outTile; //�� �ܺ��� Ÿ��
    int type;
    void Start()
    {
        // DrawMap(0, 0);
        MakeMap(0);
    }
    void FillBackground()
    {
        for (int i = -10; i < mapSize.x + 10; i++)
        {
            for (int j = -10; j < mapSize.y + 10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile[type]);
            }
        }
    }
    void FillWall()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == outTile[type])
                {

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            if (tileMap.GetTile(new Vector3Int(i - mapSize.x / 2 + x, j - mapSize.y / 2 + y, 0)) == roomTile[type])
                            {
                                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile[type]);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    void Divide(Node tree, int n)
    {
        if (n == maximumDepth) return; //���� ���ϴ� ���̿� �����ϸ� �� �������� �ʴ´�.

        //�� ���� ��쿡��

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        //���ο� ������ �� ����� ������, ���ΰ� ��ٸ� �� ��, ��� ���ΰ� �� ��ٸ� ��, �Ʒ��� �����ְ� �� ���̴�.
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //���� �� �ִ� �ִ� ���̿� �ּ� �����߿��� �������� �� ���� ����
        if (tree.nodeRect.width >= tree.nodeRect.height) //���ΰ� �� ����� ��쿡�� �� ��� ������ �� ���̸�, �� ��쿡�� ���� ���̴� ������ �ʴ´�.
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            //���� ��忡 ���� ������ 
            //��ġ�� ���� �ϴ� �����̹Ƿ� ������ ������, ���� ���̴� ������ ���� �������� �־��ش�.
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
            //���� ��忡 ���� ������ 
            //��ġ�� ���� �ϴܿ��� ���������� ���� ���̸�ŭ �̵��� ��ġ�̸�, ���� ���̴� ���� ���α��̿��� ���� ���� ���ΰ��� �� ������ �κ��� �ȴ�.
            // DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
            //�� �� �� �ΰ��� ��带 ������ ���� �׸��� �Լ��̴�.        
        }
        else
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
            //DrawLine(new Vector2(tree.nodeRect.x , tree.nodeRect.y+ split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y  + split));
            //���ΰ� �� ����� ����̴�. �ڼ��� ������ ���ο� ����.
        }
        tree.leftNode.parNode = tree; //�ڽĳ����� �θ��带 �������� ���� ����
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1); //����, ������ �ڽ� ���鵵 �����ش�.
        Divide(tree.rightNode, n + 1);//����, ������ �ڽ� ���鵵 �����ش�.
    }
    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;
        if (n == maximumDepth) //�ش� ��尡 ��������� ���� ����� �� ���̴�.
        {
            rect = tree.nodeRect;
            int width = Random.Range(rect.width / 2, rect.width - 1);
            //���� ���� �ּ� ũ��� ����� ���α����� ����, �ִ� ũ��� ���α��̺��� 1 �۰� ������ �� �� ���� ���� ������ ���� �����ش�.
            int height = Random.Range(rect.height / 2, rect.height - 1);
            //���̵� ���� ����.
            int x = rect.x + Random.Range(1, rect.width - width);
            //���� x��ǥ�̴�. ���� 0�� �ȴٸ� �پ� �ִ� ��� �������� ������,�ּڰ��� 1�� ���ְ�, �ִ��� ���� ����� ���ο��� ���� ���α��̸� �� �� ���̴�.
            int y = rect.y + Random.Range(1, rect.height - height);
            //y��ǥ�� ���� ����.
            rect = new RectInt(x, y, width, height);
            //DrawRectangle(rect);
            FillRoom(rect);
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }
    private void FillRoom(RectInt rect)
    {
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile[type]);
            }
        }
    }

    private void GenerateLoad(Node tree, int n)
    {
        if (n == maximumDepth) //���� ����� ���� �ڽ��� ����.
            return;
        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;

        for (int i = Mathf.Min(leftNodeCenter.x, rightNodeCenter.x); i <= Mathf.Max(leftNodeCenter.x, rightNodeCenter.x); i++)
        {
            tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftNodeCenter.y - mapSize.y / 2, 0), roomTile[type]);
        }

        for (int j = Mathf.Min(leftNodeCenter.y, rightNodeCenter.y); j <= Mathf.Max(leftNodeCenter.y, rightNodeCenter.y); j++)
        {
            tileMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile[type]);
        }



        GenerateLoad(tree.leftNode, n + 1); //�ڽ� ���鵵 Ž��
        GenerateLoad(tree.rightNode, n + 1);
    }

    #region �� ���� ������ �׷����� ����
   /* private void DrawMap(int x, int y) //x y�� ȭ���� �߾���ġ�� ����
    {
        //�⺻������ mapSize/2��� ���� ����ؼ� ���� �ɰǵ�, ȭ���� �߾ӿ��� ȭ���� ũ���� ���� ����� ���� �ϴ���ǥ�� ���� �� �ֱ� �����̴�.
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2); //���� �ϴ�
        lineRenderer.SetPosition(1, new Vector2(x + mapSize.x, y) - mapSize / 2); //���� �ϴ�
        lineRenderer.SetPosition(2, new Vector2(x + mapSize.x, y + mapSize.y) - mapSize / 2);//���� ���
        lineRenderer.SetPosition(3, new Vector2(x, y + mapSize.y) - mapSize / 2); //���� ���

    }
    private void DrawRectangle(RectInt rect)
    {
        LineRenderer lineRenderer = Instantiate(roomLine).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(rect.x, rect.y) - mapSize / 2); //���� �ϴ�
        lineRenderer.SetPosition(1, new Vector2(rect.x + rect.width, rect.y) - mapSize / 2); //���� �ϴ�
        lineRenderer.SetPosition(2, new Vector2(rect.x + rect.width, rect.y + rect.height) - mapSize / 2);//���� ���
        lineRenderer.SetPosition(3, new Vector2(rect.x, rect.y + rect.height) - mapSize / 2); //���� ���
    }
    private void DrawLine(Vector2 from, Vector2 to) //from->to�� �̾����� ���� �׸��� �� ���̴�.
    {
        LineRenderer lineRenderer = Instantiate(line).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }*/
    #endregion
    public void MakeMap(int i)
    {
        type = i;
        FillBackground();
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);
        GenerateRoom(root, 0);
        GenerateLoad(root, 0);
        FillWall();
    }

}