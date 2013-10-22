using UnityEngine;
using System;
using System.Collections.Generic;

public class FMeshNode : FFacetElementNode
{
	protected Color _color = Futile.white;
	protected Color _alphaColor = Futile.white;
	
	protected bool _isMeshDirty = false;

	protected FMeshData _meshData;
	protected int _previousMeshDataVersion;

	protected Vector2 _uvScale;
	protected Vector2 _uvOffset;

	protected bool _shouldUseWholeAtlas = false;

	protected FMeshNode() : base() //for overriding
	{

	}

	public FMeshNode (string elementName) : this(new FMeshData(), Futile.atlasManager.GetElementWithName(elementName))
	{
	}

	public FMeshNode (FAtlasElement element) : this(new FMeshData(), element)
	{
	}
	
	public FMeshNode (FMeshData meshData, string elementName) : this(meshData, Futile.atlasManager.GetElementWithName(elementName))
	{
	}

	public FMeshNode (FMeshData meshData, FAtlasElement element) : base()
	{
		Init(meshData, element);
	}

	protected void Init(FMeshData meshData, FAtlasElement element)
	{
		_meshData = meshData;
		_previousMeshDataVersion = _meshData.version;
		
		Init(_meshData.facetType, element,meshData.facets.Count);

		_isMeshDirty = true;
		_isAlphaDirty = true;
	}

	public override void HandleAddedToStage()
	{
		base.HandleAddedToStage();

		if(_previousMeshDataVersion < _meshData.version) 
		{
			HandleMeshDataChanged();
		}

		_meshData.SignalUpdate += HandleMeshDataChanged;
	}

	public override void HandleRemovedFromStage()
	{
		base.HandleRemovedFromStage();

		_meshData.SignalUpdate -= HandleMeshDataChanged;
	}

	public void HandleMeshDataChanged()
	{
		_previousMeshDataVersion = _meshData.version;
		_isMeshDirty = true;

		int facetCount = _meshData.facets.Count;
		if(_numberOfFacetsNeeded != facetCount)
		{
			_numberOfFacetsNeeded = facetCount;
			_stage.HandleFacetsChanged();
		}
	}
	
	override public void HandleElementChanged()
	{
		_isMeshDirty = true;

		if(_shouldUseWholeAtlas)
		{
			_uvScale = new Vector2(1.0f,1.0f);
			_uvOffset = new Vector2(0.0f,0.0f);
		}
		else 
		{
			_uvScale = new Vector2(_element.uvRect.width, _element.uvRect.height);
			_uvOffset = new Vector2(_element.uvRect.xMin, _element.uvRect.yMin);
		}
	}
	
	override public void Redraw(bool shouldForceDirty, bool shouldUpdateDepth)
	{
		bool wasMatrixDirty = _isMatrixDirty;
		bool wasAlphaDirty = _isAlphaDirty;
		
		UpdateDepthMatrixAlpha(shouldForceDirty, shouldUpdateDepth);
		
		if(shouldUpdateDepth)
		{
			UpdateFacets();
		}
		
		if(wasMatrixDirty || shouldForceDirty || shouldUpdateDepth)
		{
			_isMeshDirty = true;
		}
		
		if(wasAlphaDirty || shouldForceDirty)
		{
			_isMeshDirty = true;
			_color.ApplyMultipliedAlpha(ref _alphaColor, _concatenatedAlpha);	
		}
		
		if(_isMeshDirty) 
		{
			PopulateRenderLayer();
		}
	}
	
	override public void PopulateRenderLayer()
	{
		if(_isOnStage && _firstFacetIndex != -1) 
		{
			_isMeshDirty = false;

			if(_meshData.facetType == FFacetType.Triangle)
			{
				int vertexIndex0 = _firstFacetIndex*4;
				int vertexIndex1 = vertexIndex0 + 1;
				int vertexIndex2 = vertexIndex0 + 2;

				Vector3[] vertices = _renderLayer.vertices;
				Vector2[] uvs = _renderLayer.uvs;
				Color[] colors = _renderLayer.colors;

				List<FMeshFacet> facets = _meshData.facets;
				int facetCount = facets.Count;

				for(int f = 0; f<facetCount; f++)
				{
					FMeshFacet facet = facets[f];

					FMeshVertex[] meshVertices = facet.vertices;
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex0], meshVertices[0].pos, _meshZ);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex1], meshVertices[1].pos, _meshZ);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex2], meshVertices[2].pos, _meshZ);

					//this needs to be offset by the element uvs, so that the uvs are relative to the element (add then multiply element uv)
					uvs[vertexIndex0] = new Vector2(_uvOffset.x + meshVertices[0].uv.x * _uvScale.x,_uvOffset.y + meshVertices[0].uv.y * _uvScale.y);
					uvs[vertexIndex1] = new Vector2(_uvOffset.x + meshVertices[1].uv.x * _uvScale.x,_uvOffset.y + meshVertices[1].uv.y * _uvScale.y);
					uvs[vertexIndex2] = new Vector2(_uvOffset.x + meshVertices[2].uv.x * _uvScale.x,_uvOffset.y + meshVertices[2].uv.y * _uvScale.y);

					//could also use vertex colours here!
					colors[vertexIndex0] = _alphaColor * meshVertices[0].color;
					colors[vertexIndex1] = _alphaColor * meshVertices[1].color;
					colors[vertexIndex2] = _alphaColor * meshVertices[2].color;

					vertexIndex0 += 3;
					vertexIndex1 += 3;
					vertexIndex2 += 3;
				}

			}
			else if(_meshData.facetType == FFacetType.Quad)
			{
				int vertexIndex0 = _firstFacetIndex*4;
				int vertexIndex1 = vertexIndex0 + 1;
				int vertexIndex2 = vertexIndex0 + 2;
				int vertexIndex3 = vertexIndex0 + 3;
				
				Vector3[] vertices = _renderLayer.vertices;
				Vector2[] uvs = _renderLayer.uvs;
				Color[] colors = _renderLayer.colors;
				
				List<FMeshFacet> facets = _meshData.facets;
				int facetCount = facets.Count;
				
				for(int f = 0; f<facetCount; f++)
				{
					FMeshFacet facet = facets[f];
					
					FMeshVertex[] meshVertices = facet.vertices;
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex0], meshVertices[0].pos, _meshZ);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex1], meshVertices[1].pos, _meshZ);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex2], meshVertices[2].pos, _meshZ);
					_concatenatedMatrix.ApplyVector3FromLocalVector2(ref vertices[vertexIndex3], meshVertices[3].pos, _meshZ);
					
					//this needs to be offset by the element uvs, so that the uvs are relative to the element (add then multiply element uv)
					uvs[vertexIndex0] = new Vector2(_uvOffset.x + meshVertices[0].uv.x * _uvScale.x,_uvOffset.y + meshVertices[0].uv.y * _uvScale.y);
					uvs[vertexIndex1] = new Vector2(_uvOffset.x + meshVertices[1].uv.x * _uvScale.x,_uvOffset.y + meshVertices[1].uv.y * _uvScale.y);
					uvs[vertexIndex2] = new Vector2(_uvOffset.x + meshVertices[2].uv.x * _uvScale.x,_uvOffset.y + meshVertices[2].uv.y * _uvScale.y);
					uvs[vertexIndex3] = new Vector2(_uvOffset.x + meshVertices[3].uv.x * _uvScale.x,_uvOffset.y + meshVertices[3].uv.y * _uvScale.y);
					
					//could also use vertex colours here!
					colors[vertexIndex0] = _alphaColor * meshVertices[0].color;
					colors[vertexIndex1] = _alphaColor * meshVertices[1].color;
					colors[vertexIndex2] = _alphaColor * meshVertices[2].color;
					colors[vertexIndex3] = _alphaColor * meshVertices[3].color;
					
					vertexIndex0 += 4;
					vertexIndex1 += 4;
					vertexIndex2 += 4;
					vertexIndex3 += 4;
				}
				
			}

			_renderLayer.HandleVertsChange();
		}
	}

	virtual public Color color 
	{
		get { return _color; }
		set 
		{ 
			if(_color != value)
			{
				_color = value; 
				_isAlphaDirty = true;
			}
		}
	}

	public FMeshData meshData
	{
		get {return _meshData;}
		set 
		{
			if(_meshData != value)
			{
				_meshData = value;
				_previousMeshDataVersion = _meshData.version;
				_numberOfFacetsNeeded = _meshData.facets.Count;
				_isMeshDirty = true;
			}
		}
	}

	public bool shouldUseWholeAtlas
	{
		get
		{
			return _shouldUseWholeAtlas;
		}
		set 
		{
			if(_shouldUseWholeAtlas != value)
			{
				_shouldUseWholeAtlas = value;
				HandleElementChanged();
			}
		}
	}
}

