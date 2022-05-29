#include <stdio.h>
#include <stdlib.h>
#include <fstream>

using namespace std;
long getFileSize(FILE *file);
int ConvertInt16(unsigned char Byte1, unsigned char Byte2);
int ConvertInt32(unsigned char Byte1, unsigned char Byte2,  unsigned char Byte3,  unsigned char Byte4);
void export(int game);
int getValAtAddress(unsigned char *buffer, long Address, int size);
int getValAtCoordinate(int x, int y, int BlockStart,unsigned char *buffer, int size);
int getTile(int tileData);
int getHeight(int tileData);
int getDoors(long tileData);
int getFloorTex(long tileData);
int getWallTex(long tileData);
int getObject(long tileData);

// Get the size of a file
	long getFileSize(FILE *file)
	{
	    long lCurPos, lEndPos;
	    lCurPos = ftell(file);
	    fseek(file, 0, 2);
	    lEndPos = ftell(file);
	    fseek(file, lCurPos, 0);
	    return lEndPos;
	}
	 
	void export(int game)
	{
	    const char *filePath = "C:\\Games\\Ultima\\UW1\\DATA\\lev.ark"; 
	    unsigned char *BigEndBuf;          // Pointer to our buffered data (big endian format)
		unsigned char *lev_ark;          // Pointer to our buffered data (little endian format)
	    int NoOfBlocks;
		long AddressOfBlockStart;
		int valAtStartOfBlock;
		int x;	
		int y;
		int LevelNo;

		FILE *file = NULL;      // File pointer
		
	    if ((file = fopen(filePath, "rb")) == NULL)
	        printf("Could not open specified file\n");
	    else
	        printf ("File opened successfully\n");
	 
	    // Get the size of the file in bytes
	    long fileSize = getFileSize(file);
	 
	    // Allocate space in the buffer for the whole file
	    BigEndBuf = new unsigned char[fileSize];
		lev_ark = new unsigned char[fileSize];
	    // Read the file in to the buffer
	    fread(BigEndBuf, fileSize, 1,file);
		fclose(file);  
	    //turn it into little endian
	    for (int i = 0; i < fileSize; i++)
			{
				if (i%2 == 0)
					lev_ark[i] = BigEndBuf[i+1];
				else
					lev_ark[i] = BigEndBuf[i-1];
			}   

		//Get the number of blocks in this file.
		NoOfBlocks = ConvertInt16(lev_ark[0],lev_ark[1]);
		printf("There are %d blocks in this file.\n",NoOfBlocks);

		//Now lets loop through the levels.
		for(LevelNo = 0; LevelNo <=8; LevelNo++)
			{
			//Get the first map block
			AddressOfBlockStart = getValAtAddress(lev_ark,(LevelNo * 4) + 2,32);
			//lets get some x y co-ordinates origin is at the lower right
			printf ("\nNow Printing Height Map for level :%d.", LevelNo+1);
			for (int y=0; y<64;y++)
				{
				printf ("\n");
				for (x=0; x<64;x++)
					{
						printf("%02d",getHeight(getValAtCoordinate(x,63-y,AddressOfBlockStart,lev_ark,16)));
					}
				}
			printf ("\nNow Printing Tilemap for level :%d.", LevelNo+1);
			for (y=0; y<64;y++)
				{
				printf ("\n");
				for (x=0; x<64;x++)
					{
						printf("%d",getTile(getValAtCoordinate(x,63-y,AddressOfBlockStart,lev_ark,16)));
					}
				}
			
			printf ("\nNow Printing floor textures for level :%d.(##)", LevelNo+1);
			for (y=0; y<64;y++)
				{
				printf ("\n");
				for (x=0; x<64;x++)
					{
						printf("%02d",getFloorTex(getValAtCoordinate(x,63-y,AddressOfBlockStart,lev_ark,16)));
					}
				}

			printf ("\nNow Printing door positions for level :%d.", LevelNo+1);
			for (y=0; y<64;y++)
				{
				printf ("\n");
				for (x=0; x<64;x++)
					{
						printf("%d",getDoors(getValAtCoordinate(x,63-y,AddressOfBlockStart,lev_ark,16)));
					}
				}
			
			printf ("\nNow Printing wall textures for level :%d.(##)", LevelNo+1);
			for (y=0; y<64;y++)
				{
				printf ("\n");
				for (x=0; x<64;x++)
					{
						printf("%02d",getWallTex(getValAtCoordinate(x,63-y,AddressOfBlockStart,lev_ark,32)));
					}
				}

			printf ("\nNow Printing tile objects for level :%d.", LevelNo+1);
			for (y=0; y<64;y++)
				{
				printf ("\n");
				for (x=0; x<64;x++)
					{
						printf("%d",getObject(getValAtCoordinate(x,63-y,AddressOfBlockStart,lev_ark,32)));
					}
				}
		}
	}
	int ConvertInt16(unsigned char Byte1, unsigned char Byte2)
	{
	return Byte1 << 8 | Byte2 ;
	}

	int ConvertInt32(unsigned char Byte1, unsigned char Byte2,  unsigned char Byte3,  unsigned char Byte4)
	{
	return Byte1 << 32 | Byte2 << 16 | Byte3 << 8 | Byte4 ;
	}

	int getValAtAddress(unsigned char *buffer, long Address, int size)
	{//Gets contents of bytes the the specific integer address. int(8), int(16), int(32) per uw-formats.txt
		switch (size)
		{
		case 8:
			{return buffer[Address];}
		case 16:
			{return ConvertInt16(buffer[Address],buffer[Address+1]);}
		case 32:
			{return ConvertInt32(buffer[Address+2],buffer[Address+3],buffer[Address],buffer[Address+1]);}
		default:
			{
			printf("Invalid size entered!");
			return -1;
			}
		}
	}



int getValAtCoordinate(int x, int y, int BlockStart,unsigned char *buffer,int size)
{
	int val = getValAtAddress(buffer, BlockStart + (x*4) + (y * (4 * 64)),size);
	return val;
}


int getTile(int tileData)
{
	//gets tile data at bits 0-3 of the tile data
	return (tileData & 0x0F);
}

int getHeight(int tileData)
{//gets height data at bits 4-7 of the tile data
	return (tileData & 0xF0) >> 4;
}

int getFloorTex(long tileData)
{//gets floor texture data at bits 10-13 of the tile data
	return (tileData >>11 & 0x0F);
}

int getDoors(long tileData)
{//gets door positions at bit 15 of the tile data
	return (tileData>>15 & 0x01);
}


int getWallTex(long tileData)
{//gets wall texture data at bits 0-5 (+16) of the tile data(2nd part)
	return (tileData >>17 & 0x3F);
}

int getObject(long tileData)
{//gets object data at bits 6-15 (+16) of the tile data(2nd part)
//unverified
	return (tileData >>21);
}
	int main()
	{
	export(1);
	}