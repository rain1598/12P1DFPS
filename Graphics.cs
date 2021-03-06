using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class Graphics : Node2D{
	public class pixc{ //stores the closest viewable objects and there distance (pixel colour)
		public Lib.obj type;
		public byte bright; //luminosity
		public pixc(Lib.obj type, byte bright){
			this.type = type;
			this.bright = bright;
		}
	}
	public static pixc[] sight = new pixc[24]; //every direction of the viewport
	static Vector2 pos;
	public static byte starton = 20; //the angle of the first rectangle
	static bool reloaded = true; //tells when the frontend has to change
	public static void interpret(byte angle){ //an angle = 15 degrees
		bool xplus = false;
		bool yplus = false;
		byte area = (byte)(angle%6);
		switch(angle/6){
			case 0:
				xplus = true; yplus = true;
				if(angle%6 == 0)yplus = false;
				break;
			case 1:
				xplus = true; yplus = false;
				if(angle%6 == 0)yplus = true;
				else{area = (byte)(6-area);}
				break;
			case 2:
				xplus = false; yplus = false;
				break;
			case 3:
				xplus = false; yplus = true;
				if(angle%6 != 0)area = (byte)(6-area);
				break;
		}
		switch(area){	
			case 0:
				search(angle, 0, true, xplus, yplus);
				break;
			case 4:
				search(angle, 2, xplus, yplus, true);
				break;
			case 5:
				search(angle, 1, xplus, yplus, true);
				break;
			default:
				search(angle, area, xplus, yplus, false);
				break;
		}
	}
	static void search(byte angle, byte arrId, bool Xplus, bool Yplus, bool sxy){
		foreach(Vector2 e in Lib.Drivers[arrId]){
			Vector2 t = e;
			if(!Xplus)t.x = 0-e.x;
			if(!Yplus)t.y = 0-e.y;
			t += pos;
			if(sxy)t = new Vector2(t.y, t.x);

			try{
				if(Player.map[(int)t.x, (int)t.y] != Lib.obj.empty){
					sight[angle] = new pixc(Player.map[(int)t.x, (int)t.y], big(e)); //lum = 11-mathmax
					return;
				}
			}catch(System.IndexOutOfRangeException){
				sight[angle] = new pixc(Lib.obj.wall, big(e));
				return;
			}
		}
		sight[angle] = new pixc(Lib.obj.empty, 0);
	}
	static byte big(Vector2 et){ //calculates whether x or y is the biggest, then calculates their brightness
		float biggest;
		if(et.x == 0)et.x = 1;
		if(et.y == 0)et.y = 1;
		biggest = et.x > et.y ? et.x : et.y;
		biggest = 16 - biggest;
		biggest *= 17;
		return Convert.ToByte(biggest);
	}
	public static void reload(){//reloads every viewport angle
		pos = Player.Pos;
		for(byte i = 0; i <= 23; i++) {
			interpret(i);
		}
		reloaded = true;
	}
	public override void _Process(float delta){
		if(!reloaded)return;
		byte temp = 0;
		for(int i = starton; i < starton+9; i++){
			render(GetChild<ColorRect>(temp), (i % 24));
			temp++;
		}
	}
	static void render(ColorRect pixel, int rnum){ //rnum = the index in sight (render number)
		Color value = new Color();
		try{
			Lib.obj2c.TryGetValue(sight[rnum].type, out value);
			pixel.Color = Color.Color8((byte)value.r8, (byte)value.g8, (byte)value.b8, sight[rnum].bright);
		}catch(NullReferenceException){}
	}
}
