using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using SiraUtil.Converters;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace FightSabers.Settings
{
	internal class PluginConfig
	{
		public event EventHandler? ConfigChanged;

		public virtual bool Enabled { get; set; } = true;

		[UseConverter(typeof(Vector3Converter))]
		public virtual Vector3 PanelPosition { get; set; } = new(0, 3.85f, 2.4f);

		[UseConverter(typeof(Vector3Converter))]
		public virtual Vector3 PanelRotation { get; set; } = new(-15, 0, 0);

		public virtual void OnReload()
		{
			// This is called whenever the config file is reloaded from disk.
			// Use it to tell all of your systems that something has changed.

			// This is called off of the main thread, and is not safe to interact with Unity in
			ConfigChanged?.Invoke(this, EventArgs.Empty);
		}

		public virtual void Changed()
		{
			// this is called whenever one of the virtual properties is changed
			// can be called to signal that the content has been changed
		}

		public virtual IDisposable ChangeTransaction => null!;
	}
}