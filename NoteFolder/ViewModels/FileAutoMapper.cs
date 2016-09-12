using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NoteFolder.Models;
using AutoMapper;

namespace NoteFolder.ViewModels {
	public static class FileAutoMapper {
		public static void Map() {
			Mapper.CreateMap<File, FileVM>();
		}
	}
}