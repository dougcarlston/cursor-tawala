// $Workfile: UndoManager.cs $
// $Revision: 6 $	$Date: 12/16/07 6:01p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tawala.UndoSupport
{
	public class UndoManager
	{
		private const bool USE_COMPRESSION = false;

		/// <summary>
		/// Clears the undo and redo stacks and resets important variables to their default states.
		/// </summary>
		public void Reset()
		{
			resetUndoState();
			resetRedoState();
		}

		// stack to preserve mementos for implementing undo
        private Stack<MemoryStream> undoStack = new Stack<MemoryStream>();

		/// <summary>
		/// Adds memento to undo stack and clears redo stack
		/// </summary>
		public void Remember(IMemento memento)
		{
			resetRedoState();
			undoSave(memento);
		}

        /// <summary>
        /// Preserves memento on undo stack
        /// </summary>
		private void undoSave(IMemento memento)
        {
			undoActionText = memento.ActionText;

			MemoryStream serializedMemento = serializeMemento(memento, USE_COMPRESSION);
			undoStack.Push(serializedMemento);
			undoStackSize += serializedMemento.Length;
		}

		private long undoStackSize;
        
        /// <summary>
        /// Returns true if there is something to undo
        /// </summary>
        public bool CanUndo
        {
            get { return (undoStack.Count > 0); }
        }

		/// <summary>
		/// Restores memento from undo stack after preserving prior memento on redo stack
		/// </summary>
		public IMemento Undo(IMemento priorMemento)
		{
			redoSave(priorMemento);

			IMemento memento = undo();

			return memento;
		}


        /// <summary>
        /// Gets memento from top of undo stack without popping
        /// </summary>
		private IMemento undoPeek()
        {
			return (undoStack.Count > 0 ? deserializeMemento(undoStack.Peek(), USE_COMPRESSION) : null);
		}

		/// <summary>
		/// Returns the text associated with the current undo action
		/// </summary>
		public string UndoActionText
		{
			get
			{
				return undoActionText;
			}
		}

		private string undoActionText = "";

		/// <summary>
		/// Restores memento from undo stack
		/// </summary>
		private IMemento undo()
		{
			IMemento memento = null;
			undoActionText = "";

			if (undoStack.Count > 0)
			{
				MemoryStream serializedMemento = undoStack.Pop();
				undoStackSize -= serializedMemento.Length;
				memento = deserializeMemento(serializedMemento, USE_COMPRESSION);

				serializedMemento.Dispose();

				if (undoStack.Count > 0)
				{
					undoActionText = undoPeek().ActionText;
				}
			}

			return memento;
		}

        
        // stack to preserve mementos for implementing redo
        private Stack<MemoryStream> redoStack = new Stack<MemoryStream>();

        /// <summary>
        /// Returns true if there is something to redo
        /// </summary>
        public bool CanRedo
        {
            get { return (redoStack.Count > 0); }
        }

        /// <summary>
		/// Restores memento from redo stack
        /// </summary>
		private IMemento redo()
        {
			IMemento memento = null;
			redoActionText = "";

			if (redoStack.Count > 0)
			{
				MemoryStream serializedMemento = redoStack.Pop();
				redoStackSize -= serializedMemento.Length;
				memento = deserializeMemento(serializedMemento, USE_COMPRESSION);

				serializedMemento.Dispose();

				if (redoStack.Count > 0)
				{
					redoActionText = redoPeek().ActionText;
				}
			}

			return memento;
		}

		/// <summary>
		/// Restores memento from redo stack after preserving prior memento on undo stack
		/// </summary>
		public IMemento Redo(IMemento priorMemento)
		{
			undoSave(priorMemento);
			
			IMemento memento = redo();

			return memento;
		}

        /// <summary>
		/// Gets memento from top of redo stack without popping
        /// </summary>
		private IMemento redoPeek()
        {
			return (redoStack.Count > 0 ? deserializeMemento(redoStack.Peek(), USE_COMPRESSION) : null);
		}

		/// <summary>
		/// Returns the text associated with the current redo action
		/// </summary>
		public string RedoActionText
		{
			get
			{
				return redoActionText;
			}
		}

		private string redoActionText = "";

		/// <summary>
		/// Preserves memento on redo stack
		/// </summary>
		private void redoSave(IMemento memento)
		{
			redoActionText = memento.ActionText;

			MemoryStream serializedMemento = serializeMemento(memento, USE_COMPRESSION);
			redoStack.Push(serializedMemento);
			redoStackSize += serializedMemento.Length;
		}

		private long redoStackSize;

		/// <summary>
		/// Serializes memento to memory stream
		/// </summary>
		private static MemoryStream serializeMemento(IMemento memento)
		{
			IFormatter formatter = new BinaryFormatter();

			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, memento);
			return stream;
		}

		/// <summary>
		/// Serializes memento to memory stream
		/// </summary>
		private static MemoryStream serializeMemento(IMemento memento, bool useCompression)
		{
			if (useCompression)
			{
				MemoryStream stream = new MemoryStream();

				IFormatter formatter = new BinaryFormatter();

				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Compress, true))
				{
					formatter.Serialize(deflateStream, memento);
				}

				return stream;
			}
			else
			{
				return serializeMemento(memento);
			}
		}

		/// <summary>
		/// Deserializes memento from memory stream
		/// </summary>
		private static IMemento deserializeMemento(MemoryStream stream)
		{
			IFormatter formatter = new BinaryFormatter();
			stream.Position = 0;

			return (IMemento)formatter.Deserialize(stream);
		}

		/// <summary>
		/// Deserializes memento from memory stream
		/// </summary>
		private static IMemento deserializeMemento(MemoryStream stream, bool useCompression)
		{
			if (useCompression)
			{
				IMemento memento;

				stream.Position = 0;

				IFormatter formatter = new BinaryFormatter();

				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
				{
					memento = (IMemento)formatter.Deserialize(deflateStream);
				}

				return memento;
			}
			else
			{
				return deserializeMemento(stream);
			}
		}

		private void resetUndoState()
		{
			while (undoStack.Count > 0)
			{
				MemoryStream stream = undoStack.Pop();
				stream.Dispose();
			}

			undoActionText = "";
			undoStackSize = 0;
		}

		private void resetRedoState()
		{
			while (redoStack.Count > 0)
			{
				MemoryStream stream = redoStack.Pop();
				stream.Dispose();
			}

			redoActionText = "";
			redoStackSize = 0;
		}


	}
}
