﻿using System;
using System.IO;
using System.Collections;
using CSCore.Tags.ID3;
using NVorbis;
using JAudioTags;
using System.Runtime.Serialization.Formatters;

namespace aplicacion_musica
{
    public class LectorMetadatos
    {
        private readonly VorbisReader _vorbisReader = null;
        private readonly FLACFile _FLACfile;
        private readonly ID3v2QuickInfo _mp3iD3 = null;
        public string Artista { get; private set; }
        public string Titulo { get; private set; }
        public int Pista { get; private set; }
        public string Album { get; private set; }
        public int Año { get; private set; }
        public LectorMetadatos(string s)
        {
            switch (Path.GetExtension(s))
            {
                case ".mp3":
                    ID3v2 mp3tag = ID3v2.FromFile(s);
                    _mp3iD3 = new ID3v2QuickInfo(mp3tag);
                    if (_mp3iD3.Artist == "")
                        Artista = "S/N";
                    else
                        Artista = _mp3iD3.Artist;
                    Titulo = _mp3iD3.Title;
                    Album = _mp3iD3.Album;
                    Pista = _mp3iD3.TrackNumber ?? 0;
                    Año = _mp3iD3.Year ?? 0;
                    break;
                case ".flac":
                    _FLACfile = new FLACFile(s, true);
                    Artista = _FLACfile.ARTIST;
                    Titulo = _FLACfile.TITLE;
                    Album = _FLACfile.ALBUM;
                    Pista = Convert.ToInt32(_FLACfile.TRACKNUMBER);
                    Año = 0;
                    break;
                case ".ogg":
                    _vorbisReader = new VorbisReader(s);
                    foreach (String meta in _vorbisReader.Comments)
                    {
                        if (meta.Contains("TITLE="))
                            Titulo = meta.Replace("TITLE=", "");
                        else if (meta.Contains("TITLE=".ToLower()))
                            Titulo = meta.Replace("title=", "");
                        else if (meta.Contains("ARTIST="))
                            Artista = meta.Replace("ARTIST=", "");
                        else if (meta.Contains("ARTIST=".ToLower()))
                            Artista = meta.Replace("artist=", "");
                        else if (meta.Contains("TRACKNUMBER="))
                            Pista = Convert.ToInt32(meta.Replace("TRACKNUMBER=", ""));
                        else if (meta.Contains("tracknumber="))
                            Pista = Convert.ToInt32(meta.Replace("tracknumber=", ""));
                        else if(meta.Contains("ALBUM="))
                            Album = meta.Replace("ALBUM=", "");
                        else if (meta.Contains("album="))
                            Album = meta.Replace("album=", "");
                    }
                    Año = 0;
                    Cerrar();
                    break;
                default:
                    break;
            }
        }
        private void Cerrar()
        {
            if (_vorbisReader != null)
                _vorbisReader.Dispose();
        }
        public bool Evaluable()
        {
            return ((Artista != null) && (Titulo != null)) ? true : false;
        }
    }
}
