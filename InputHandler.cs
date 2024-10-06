﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_CsProg_ITHS.NET;
internal class InputHandler
{
	public static InputHandler Instance { get; private set; } = new();
    private InputHandler(){}

	internal Task<ConsoleKeyInfo> InputListener = default!;
	internal ConcurrentQueue<ConsoleKeyInfo> KeyBuffer { get; private set; } = new();
	private HashSet<ConsoleKey> _keys = new();
    public bool ReadAllKeys { get; set; } = false;

	private bool running = false;

	internal ConsoleKeyInfo Start()
	{
		ConsoleKeyInfo k = default;
		running = true;
		while (running)
		{
			k = Console.ReadKey(true);
			if(!running) break;

			if (ReadAllKeys || _keys.Contains(k.Key))
			{
				KeyBuffer.Enqueue(k);
			}
		}
		return k;
	}

	internal void Stop()
	{
		running = false;
	}
	internal void AddKeyListener(ConsoleKey key)
	{
		_keys.Add(key);
	}
	internal ConsoleKeyInfo AwaitNextKey()
	{
		running = false;
		var key = InputListener.Result;
		
		InputListener = Task.Run(Start);

		return key;
	}
}
