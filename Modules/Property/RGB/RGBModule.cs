﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Property;
using Vixen.Module.Effect;
using Vixen.Sys;
using CommonElements.ColorManagement.ColorModels;
using CommandStandard.Types;
using System.Drawing;

namespace VixenModules.Property.RGB
{
	public class RGBModule : PropertyModuleInstanceBase
	{
		private RGBData _data;

		public override void SetDefaultValues()
		{
			List<ChannelNode> channels = new List<ChannelNode>(Owner.Children);
			if (channels.Count == 3) {
				_data.RGBType = RGBModelType.eIndividualRGBChannels;
				_data.RedChannelNode = channels[0].Id;
				_data.GreenChannelNode = channels[1].Id;
				_data.BlueChannelNode = channels[2].Id;
			} else {
				_data.RGBType = RGBModelType.eSingleRGBChannel;
			}
		}

		public override void Setup()
		{
			List<ChannelNode> channels = new List<ChannelNode>(Owner.Children);
			using (RGBSetup form = new RGBSetup(_data, channels.ToArray())) {
				form.ShowDialog();
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = value as RGBData; }
		}

		public ChannelData RenderColorToCommands(CommonElements.ColorManagement.ColorModels.RGB color, Level level)
		{
			ChannelData result = new ChannelData();

			HSV finalColor = HSV.FromRGB(color);
			finalColor.V = level;

			// if it's all getting dumped out each channel, just make 'smart' RGB commands for them
			if (_data.RGBType == RGBModelType.eSingleRGBChannel)
			{
				CommandStandard.Types.Color fullColorParameter = finalColor.ToRGB().ToArgb();
				PopulateChannelDataWithCommandsForChannelNode(result, Owner, CommandStandard.Standard.Lighting.Polychrome.SetColor.Id, fullColorParameter);
			}
			// otherwise, we're breaking it up by channel, so split the color up into discrete components
			else
			{
				Level R = finalColor.ToRGB().R;
				Level G = finalColor.ToRGB().G;
				Level B = finalColor.ToRGB().B;

				// populate the red channel(s) with a setlevel of the red value
				ChannelNode redNode = ChannelNode.GetChannelNode(_data.RedChannelNode);
				if (redNode != null) {
					PopulateChannelDataWithCommandsForChannelNode(result, redNode, CommandStandard.Standard.Lighting.Monochrome.SetLevel.Id, R);
				}
				
				// populate the green channel(s) with a setlevel of the green value
				ChannelNode greenNode = ChannelNode.GetChannelNode(_data.GreenChannelNode);
				if (greenNode != null) {
					PopulateChannelDataWithCommandsForChannelNode(result, greenNode, CommandStandard.Standard.Lighting.Monochrome.SetLevel.Id, G);
				}
				
				// populate the blue channel(s) with a setlevel of the blue value
				ChannelNode blueNode = ChannelNode.GetChannelNode(_data.BlueChannelNode);
				if (blueNode != null) {
					PopulateChannelDataWithCommandsForChannelNode(result, blueNode, CommandStandard.Standard.Lighting.Monochrome.SetLevel.Id, B);
				}
			}

			return result;
		}

		private void PopulateChannelDataWithCommandsForChannelNode(ChannelData data, ChannelNode node, CommandStandard.CommandIdentifier identifier, object value)
		{
			foreach (Channel c in node) {
				Command newCommand = new Command(
					TimeSpan.Zero,		// default to 0 for the start and duration, the receiving effect can clean it up how it wants
					TimeSpan.Zero,
					identifier,
					new object[] { value }
					);
				data.AddCommandForChannel(c.Id, newCommand);
			}

		}

		public static ChannelData RenderColorToCommandsForNode(ChannelNode node, CommonElements.ColorManagement.ColorModels.RGB color, Level level)
		{
			RGBModule instance = node.Properties.Get(RGBDescriptor.ModuleID) as RGBModule;
			if (instance == null)
			{
				VixenSystem.Logging.Warning("Invalid attempt to render RGB color on node '" + node.Name + "', ID " + node.Id);
				return null;
			}

			return instance.RenderColorToCommands(color, level);
		}
	}
}
