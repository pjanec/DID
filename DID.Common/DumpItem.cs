using System;
using System.Collections.Generic;
using System.Numerics;

namespace DID.Common
{
	/// <summary>
	/// Server-side full implementation for keeping the dumped values
	/// </summary>
	public class DumpItem : Disposable, IDumpItem
	{
		public int _i;

		
		//
		// Write-Back support
		//

		#region Write-Back support

		public Action OnWriteBackAction; // called when a value is changed from remote client

		// we always set this to a function that updates our c# value from c++
		//Action _implicitWriteBackAction;
		bool _valueWrittenBack = false;	// set when the value was updated via a writeBack operation from the remote client 
		bool _valueWritableBack = false;
		protected bool WasValueWrittenBack
		{
			get
			{
				// mark as writable on first
				if( !_valueWritableBack )
				{
					_valueWritableBack = true;
				}

				if( _valueWrittenBack )
				{
					HandleWriteBack();
				}

				return _valueWrittenBack;
			}
			set
			{
				_valueWrittenBack=value;
			}
		}

		protected void HandleWriteBack()
		{
			// this flag is checked when dumpig
			_valueWrittenBack = true;

			// Note: C# writeback delegate NOT called here, but after we change the source value
			//OnWriteBackAction?.Invoke();
		}

		#endregion

		//
		// Action support
		//

		public Action OnAction { get; set; }// called when an action is fired from remote client

		//
		// Value and Type support
		//

		protected EDIType _Type;
		public EDIType Type
		{
			get { return _Type; }
			set
			{
				if( _Type != value )
				{
					_Type = value;
				}
			}
		}

		protected EValueType _ValueType;
		public EValueType ValueType
		{
			get { return _ValueType; }
		}

		protected Int64 _ValInt;
		public Int64 ValInt
		{
			get { return _ValInt; }
			set
			{
				if( _ValueType != EValueType.Int )
				{
					_ValueType = EValueType.Int;
					_ValInt	= value;
				}
				else
				if( _ValInt != value )
				{
					_ValInt	= value;
				}
			}
		}

		protected double _ValDouble;
		public double ValDouble
		{
			get { return _ValDouble; }
			set
			{
				if( _ValueType != EValueType.Double )
				{
					_ValueType = EValueType.Double;
					_ValDouble	= value;
				}
				else
				if( _ValDouble != value )
				{
					_ValDouble	= value;
				}
			}
		}

		protected string _ValString;
		public string ValString
		{
			get { return _ValString; }
			set
			{
				if( _ValueType != EValueType.String )
				{
					_ValueType = EValueType.String;
					_ValString = value;
				}
				else
				if( _ValString != value )
				{
					_ValString = value;
				}
			}
		}

		protected Vector4 _ValVector4;
		public Vector4 ValVector4
		{
			get { return _ValVector4; }
			set
			{
				if( _ValueType != EValueType.Vector4 )
				{
					_ValueType = EValueType.Vector4;
					_ValVector4 = value;
				}
				else
				if( _ValVector4 != value )
				{
					_ValVector4 = value;
				}
			}
		}

		protected Quaternion _ValQuaternion;
		public Quaternion ValQuaternion
		{
			get { return _ValQuaternion; }
			set
			{
				if( _ValueType != EValueType.Quaternion )
				{
					_ValueType = EValueType.Quaternion;
					_ValQuaternion = value;
				}
				else
				if( _ValQuaternion != value )
				{
					_ValQuaternion = value;
				}
			}
		}

		protected string _Name;
		public string Name
		{
			get { return _Name; }
			set
			{
				if( _Name != value )
				{
					_Name = value;
				}
			}
		}
			
		protected string _TypeName;
		public string TypeName
		{
			get { return _TypeName; }
			set
			{
				if( _TypeName != value )
				{
					_TypeName = value;
				}
			}
		}


		protected int _ArraySize;
		public int ArraySize
		{
			get { return _ArraySize; }
			set
			{
				if( _ArraySize != value )
				{
					_ArraySize = value;
				}
			}
		}


		protected int _ArrayIndex = -1;
		public int ArrayIndex
		{
			get { return _ArrayIndex; }
			set
			{
				if( _ArrayIndex != value )
				{
					_ArrayIndex = value;
				}
			}
		}

		public int ArrayShowFrom { get; set; }
		public int ArrayShowTo { get; set; }


		protected ITopic _Topic;
		public ITopic Topic
		{
			get
			{
				return _Topic;
			}
			set{
				_Topic = value;
			}

		}

		public DumpItem(ITopic topic=null)
		{
			_Topic = topic;
		}

		/// <summary>
		/// Creates new DI linked to same topic	- a shortcut for "new DumpItem(this.Topic)"
		/// (saves one PInvoke call and one topic wrapper creation)
		/// </summary>
		/// <returns></returns>
		public DumpItem CreateDI()
		{
			var newDI = new DumpItem( Topic );
			return newDI;
		}

		protected override void Dispose(bool disposing)
		{
			if( !IsDisposed )
			{
				if( Children != null )
				{
					// destroy children first
					foreach( var i in Children )
					{
						i.Dispose();
					}
					Children.Clear();
				}

				_Type = EDIType.Invalid;
				_ValueType = EValueType.Invalid;
			}
			base.Dispose(disposing);
		}

		// We synchronize this with the underlying C++ DumpItem
		// Use ChildrenGetAt, ChildrenSetAt, ChildrenResize to acess this
		protected List<DumpItem> Children;

		/// <summary>
		/// Adds new slots as null
		/// </summary>
		/// <param name="newSize"></param>
		protected void ChildrenExtend( int newIndex )
		{
			if( Children == null )
			{
				Children = new List<DumpItem>( newIndex );
			}

			var origChildrenCount = Children.Count;
			
			// create missing subitems as null
			for( int i=origChildrenCount; i <= newIndex; i++ )
			{
				Children.Add( null );
			}
		}

		public void ChildrenSetAt( int index, DumpItem di )
		{
			Children[index] = di;
		}

		public DumpItem ChildrenGetAt( int index )
		{
			return Children[index];
		}


		public DumpItem GetOrCreateSubitem( string name )
		{
			int index = _i++;

			// extend the Children array if not big enough, init the new items to null
			ChildrenExtend( index );

			DumpItem subDI = ChildrenGetAt(index);
			if( subDI == null )
			{
				subDI = CreateDI();
				subDI.Name = name;
				ChildrenSetAt( index, subDI );
			}

			return subDI;
		}

		public void DestroySubitem( int index )
		{
			if( index >= Children.Count )
				return;

			var di = Children[index];
			ChildrenSetAt( index, null );

			di.Dispose();
		}

		public void RemoveExtraSubitems( int indexFrom )
		{
			for( int diIdx = indexFrom; diIdx < Children.Count; diIdx++ )
			{
				DestroySubitem( diIdx );
			}

			// shring the array
			Children.RemoveRange( indexFrom, Children.Count-indexFrom );
		}

		public void RemoveExtraSubitems( )
		{
			RemoveExtraSubitems( this._i );
		}



		public void SetupHeader( EDIType diType,  string typeName )
		{
			_i = 0;
			Type = diType;
			TypeName = typeName;
		}

		// fluent API for settiong actions

		public DumpItem Actionable( Action cb )
		{
			OnAction = cb;
			return this;
		}

		public DumpItem WriteBackActionable( Action cb )
		{
			OnWriteBackAction = cb;
			return this;
		}

		
		public void DumpInt( Int64 x )
		{
			Type = EDIType.Primitive;
			ValInt = x;
		}

		public bool DumpInt( ref Int64 x )
		{
			Type = EDIType.Primitive;
			if( WasValueWrittenBack )
			{
				x = ValInt;
				WasValueWrittenBack = false;
				return true;
			}
			else
			{
				ValInt = x;
				return false;
			}
		}

		public void DumpDouble( double x )
		{
			Type = EDIType.Primitive;
			ValDouble = x;
		}

		public bool DumpDouble( ref double x )
		{
			Type = EDIType.Primitive;
			if( WasValueWrittenBack )
			{
				x = ValDouble;
				WasValueWrittenBack = false;
				return true;
			}
			else
			{
				ValDouble = x;
				return false;
			}
		}

		public void DumpString ( string x )
		{
			Type = EDIType.Primitive;
			ValString = x;
		}

		public bool DumpString ( ref string x )
		{
			Type = EDIType.Primitive;
			if( WasValueWrittenBack )
			{
				x = ValString;
				WasValueWrittenBack = false;
				return true;
			}
			else
			{
				ValString = x;
				return false;
			}
		}

		public void DumpVector4 ( Vector4 x )
		{
			Type = EDIType.Primitive;
			ValVector4 = x;
		}

		public bool DumpVector4 ( ref Vector4 x )
		{
			Type = EDIType.Primitive;
			if( WasValueWrittenBack )
			{
				x = ValVector4;
				WasValueWrittenBack = false;
				return true;
			}
			else
			{
				ValVector4 = x;
				return false;
			}
		}

		public void DumpQuaternion( Quaternion x )
		{
			Type = EDIType.Primitive;
			ValQuaternion = x;
		}

		public bool DumpQuaternion( ref Quaternion x )
		{
			Type = EDIType.Primitive;
			if( WasValueWrittenBack )
			{
				x = ValQuaternion;
				WasValueWrittenBack = false;
				return true;
			}
			else
			{
				ValQuaternion = x;
				return false;
			}
		}





	};

}
