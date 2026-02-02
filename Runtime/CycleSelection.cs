using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class CycleSelection<T> : MonoBehaviour
{
	[Header("Settings")]
	public DataList<T> dataList;

	[Header("Components")]
	public TMP_Text labelTextMesh;

	[Header("Events")]
	public UnityEventInt onIndexChanged;


	private int _index;
	public int index
	{
		get => _index;
		set
		{
			_index = Mathf.Clamp(value, 0, dataList.list.Count - 1);

			UpdateLabel();

			onIndexChanged?.Invoke(_index);
		}
	}


	public void Initialize(int index)
	{
		_index = Mathf.Clamp(index, 0, dataList.list.Count);

		UpdateLabel();
	}

	public void CycleNext()
	{
		index = index + 1 % dataList.list.Count;
	}

	public void CyclePrevious()
	{
		int index = this.index - 1 % dataList.list.Count;

		if (index < 0)
			index = dataList.list.Count;

		this.index = index;
	}


	protected virtual void UpdateLabel() => labelTextMesh.text = dataList.list[index].ToString();
}