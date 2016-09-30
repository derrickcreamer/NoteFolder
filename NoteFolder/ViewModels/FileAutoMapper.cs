using System;
using AutoMapper;
using NoteFolder.Models;

namespace NoteFolder.ViewModels {
	public static class FileAutoMapper {
		public static void Map() {
			Mapper.CreateMap<File, FileVM>();
			Mapper.CreateMap<FileVM, File>();
		}
	}
}