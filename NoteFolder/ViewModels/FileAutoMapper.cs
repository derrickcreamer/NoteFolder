using System;
using NoteFolder.Models;
using AutoMapper;

namespace NoteFolder.ViewModels {
	public static class FileAutoMapper {
		public static void Map() {
			Mapper.CreateMap<File, FileVM>();
			Mapper.CreateMap<FileVM, File>();
		}
	}
}