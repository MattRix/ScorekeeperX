
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;


public class PRAtlasElement
{
	public PRAtlasGenerator generator;

	public string name;
	public string filePath;
	public Texture2D texture;
	public PRRect packedRect;
	public bool shouldTrim = false;
	public bool shouldRotate = false;
	
	public int padding;
	public int extrude;
	
	//the size of the original texture, unscaled
	public int sourceFullWidth;
	public int sourceFullHeight;
	
	//the size of the original texture, scaled
	public int scaledFullWidth;
	public int scaledFullHeight;
	
	//the rect of the trimmed area, unscaled 
	public int sourceTrimX;
	public int sourceTrimY;
	public int sourceTrimWidth;
	public int sourceTrimHeight;
	
	//the rect of the trimmed area, scaled (this will be put into the atlas data)
	public int scaledTrimX;
	public int scaledTrimY;
	public int scaledTrimWidth;
	public int scaledTrimHeight;
	
	//expandedWidth = (scaledTrimWidth + padding + extrude)
	//this is the size that is passed into the atlas rect packer
	public int expandedWidth;
	public int expandedHeight;
	
	
	public PRAtlasElement(PRAtlasGenerator generator, string name)
	{
		this.generator = generator;
		this.name = name;
	}
	
	public string GetJSONString()
	{
		StringBuilder stringBuilder = new StringBuilder("\""+name+"\":\n{\n");
		
		//scaledTrimWidth and packedRect.width should always be the same (for height too)

		//the standard atlas data format assumes 0,0 is in the top left
		//but our coordinates are in bottom left form, so we need to flip the y coord
		int flippedPackedY = (generator.atlasHeight - packedRect.y) - packedRect.height; 
		//NOTE: this needs to be fixed to take into account the expansion

		//the coordinates within the atlas texture
		stringBuilder.Append("\t\"frame\": {\"x\":"+packedRect.x+",\"y\":"+flippedPackedY+",\"w\":"+packedRect.width+",\"h\":"+packedRect.height+"},\n");  
		 
		//this tool doesn't support rotations (though it could in the future)
		stringBuilder.Append("\t\"rotated\": "+(shouldRotate?"true":"false")+",\n");
		
		//whether it has been trimmed or not
		stringBuilder.Append("\t\"trimmed\": "+(shouldTrim?"true":"false")+",\n"); 

		int flippedScaledTrimmedY = (scaledFullHeight - scaledTrimY) - scaledTrimHeight; 

		//the trimmed coordinates within the untrimmed rect (this y will need to be flipped)
		stringBuilder.Append("\t\"spriteSourceSize\": {\"x\":"+scaledTrimX+",\"y\":"+flippedScaledTrimmedY+",\"w\":"+scaledTrimWidth+",\"h\":"+scaledTrimHeight+"},\n"); 
		
		//the original untrimmed size
		stringBuilder.Append("\t\"sourceSize\": {\"w\":"+scaledFullWidth+",\"h\":"+scaledFullHeight+"}\n"); 
		
		stringBuilder.Append("}");
		
		return stringBuilder.ToString();
	}
}
