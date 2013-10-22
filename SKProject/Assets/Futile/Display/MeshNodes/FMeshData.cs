
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class FMeshData
{
	public List<FMeshFacet> facets;
	public FFacetType facetType;
	public int version = 0;

	public delegate void UpdateDelegate();
	
	public event UpdateDelegate SignalUpdate;

	public FMeshData()
	{

	}

	public FMeshData(List<FMeshFacet> inFacets)
	{
		facets = inFacets;

		if(facets.Count != 0)
		{
			facetType = facets[0].facetType;
		}

	}

	public FMeshData(params FMeshFacet[] inFacets)
	{
		facets = new List<FMeshFacet>(inFacets);
		
		if(facets.Count != 0)
		{
			facetType = facets[0].facetType;
		}
	}

	public FMeshFacet AddFacet(FMeshFacet facet)
	{
		if(facets == null)
		{
			facets = new List<FMeshFacet>();
			facetType = facet.facetType;
		}

		if(facetType != facet.facetType) //check if the facet type is different from what we already have
		{
			Debug.LogError("You can't mix facet types in FMeshData!");
		}

		facets.Add(facet);

		return facet;
	}

	public FMeshTriangle AddTriangle()
	{
		FMeshTriangle triangle = new FMeshTriangle();
		AddFacet(triangle);
		return triangle;
	}

	public FMeshTriangle AddTriangle(FMeshTriangle triangle)
	{
		AddFacet(triangle);
		return triangle;
	}

	public FMeshQuad AddQuad()
	{
		FMeshQuad quad = new FMeshQuad();
		AddFacet(quad);
		return quad;
	}
	
	public FMeshQuad AddQuad(FMeshQuad quad)
	{
		AddFacet(quad);
		return quad;
	}

	public void MarkChanged()
	{
		version++; //things that use meshdata will check the version
		if(SignalUpdate != null) SignalUpdate();
	}
}

public class FMeshFacet
{
	public FFacetType facetType;

	public FMeshVertex[] vertices;

	public FMeshFacet SetVertex(int index, float x, float y, float u, float v)
	{
		FMeshVertex vertex = vertices[index];

		vertex.pos = new Vector2(x,y);
		vertex.uv = new Vector2(u,v);
		
		return this; //for chaining
	}

	public FMeshFacet SetVertex(int index, float x, float y, float u, float v, Color color)
	{
		FMeshVertex vertex = vertices[index];
		
		vertex.pos = new Vector2(x,y);
		vertex.uv = new Vector2(u,v);
		vertex.color = color;
		
		return this; //for chaining
	}

	public FMeshFacet SetVertexPos(int index, float x, float y)
	{
		vertices[index].pos = new Vector2(x,y);
		return this; //for chaining
	}

	public FMeshFacet SetVertexPos(int index, Vector2 pos)
	{
		vertices[index].pos = pos;
		return this; //for chaining
	}

	public FMeshFacet SetVertexUV(int index, float u, float v)
	{
		vertices[index].uv = new Vector2(u,v);
		return this; //for chaining
	}

	public FMeshFacet SetVertexUV(int index, Vector2 uv)
	{
		vertices[index].uv = uv;
		return this; //for chaining
	}

	public FMeshFacet SetVertexColor(int index, Color color)
	{
		vertices[index].color = color;

		return this; //for chaining
	}

	public FMeshFacet SetColorForAllVertices(Color color)
	{
		int count = vertices.Length;

		for(int v = 0; v<count; v++)
		{
			vertices[v].color = color;	
		}

		return this; //for chaining
	}

	public FMeshFacet OffsetPos(float offsetX, float offsetY)
	{
		int count = vertices.Length;
		
		for(int v = 0; v<count; v++)
		{
			vertices[v].pos += new Vector2(offsetX,offsetY);	
		}
		
		return this; //for chaining
	}
}

public class FMeshQuad : FMeshFacet
{
	public FMeshQuad()
	{
		facetType = FFacetType.Quad;
		vertices = new FMeshVertex[] {new FMeshVertex(),new FMeshVertex(),new FMeshVertex(),new FMeshVertex()};
	}

	public FMeshQuad SetPosRect(float leftX, float bottomY, float width, float height)
	{
		vertices[0].pos = new Vector2(leftX,bottomY+height);
		vertices[1].pos = new Vector2(leftX+width,bottomY+height);
		vertices[2].pos = new Vector2(leftX+width,bottomY);
		vertices[3].pos = new Vector2(leftX,bottomY);
		
		return this; //for chaining
	}

	public FMeshQuad SetPosRect(Rect rect)
	{
		float leftX = rect.xMin;
		float rightX = rect.xMax;
		float bottomY = rect.yMin;
		float topY = rect.yMax;
		
		vertices[0].pos = new Vector2(leftX,topY);
		vertices[1].pos = new Vector2(rightX,topY);
		vertices[2].pos = new Vector2(rightX,bottomY);
		vertices[3].pos = new Vector2(leftX,bottomY);
		
		return this; //for chaining
	}

	public FMeshQuad SetUVRect(float leftX, float bottomY, float width, float height)
	{
		vertices[0].uv = new Vector2(leftX,bottomY+height);
		vertices[1].uv = new Vector2(leftX+width,bottomY+height);
		vertices[2].uv = new Vector2(leftX+width,bottomY);
		vertices[3].uv = new Vector2(leftX,bottomY);
		
		return this; //for chaining
	}

	public FMeshQuad SetUVRect(Rect rect)
	{
		float leftX = rect.xMin;
		float rightX = rect.xMax;
		float bottomY = rect.yMin;
		float topY = rect.yMax;
		
		vertices[0].uv = new Vector2(leftX,topY);
		vertices[1].uv = new Vector2(rightX,topY);
		vertices[2].uv = new Vector2(rightX,bottomY);
		vertices[3].uv = new Vector2(leftX,bottomY);
		
		return this; //for chaining
	}

	public FMeshQuad SetUVRectFull() //creates a uv rect that takes up the whole element texture
	{
		vertices[0].uv = new Vector2(0,1);
		vertices[1].uv = new Vector2(1,1);
		vertices[2].uv = new Vector2(1,0);
		vertices[3].uv = new Vector2(0,0);
		
		return this; //for chaining
	}

	public FMeshQuad SetUVRectFromElement(FAtlasElement element) //creates a uv rect that takes up the whole element texture
	{
		float leftX = element.uvRect.xMin;
		float rightX = element.uvRect.xMax;
		float bottomY = element.uvRect.yMin;
		float topY = element.uvRect.yMax;

		vertices[0].uv = new Vector2(leftX,topY);
		vertices[1].uv = new Vector2(rightX,topY);
		vertices[2].uv = new Vector2(rightX,bottomY);
		vertices[3].uv = new Vector2(leftX,bottomY);
		
		return this; //for chaining
	}
}

public class FMeshTriangle : FMeshFacet
{
	public FMeshTriangle()
	{
		facetType = FFacetType.Triangle;
		vertices = new FMeshVertex[] {new FMeshVertex(),new FMeshVertex(),new FMeshVertex()};
	}
}


public class FMeshVertex
{
	public Vector2 pos;
	public Vector2 uv;
	public Color color = Futile.white;

	public FMeshVertex()
	{

	}

	public FMeshVertex(float x, float y, float u, float v)
	{
		this.pos = new Vector2(x,y);
		this.uv = new Vector2(u,v);
	}

	public FMeshVertex(Vector2 pos, Vector2 uv)
	{
		this.pos = pos;
		this.uv = uv;
	}
}

















