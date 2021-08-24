﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel.Composition;

namespace Viz.WrkModule.MapDefects
{

  [Export(typeof(Smv.Mef.Contracts.IWorkModuleContract))]
  public sealed class MagLabContract : Smv.Mef.Contracts.IWorkModuleContract
  {
    private readonly ImageSource largeGlyph;
    private Smv.MVVM.Commands.DelegateCommand runModuleCommand;

    public event EventHandler<Smv.RibbonUserUI.RibbonUIEventArgs> RunEvent;
    public string FriendlyName { get; set; }
    public string Version
    {
      get { return Smv.Utils.Etc.GetAssemblyVersion(System.Reflection.Assembly.GetExecutingAssembly()); }
    }

    public string Id
    {
      get { return ModuleConst.ModuleId; }
    }

    public UserControl CreateContent(Window owner)
    {
      return null;
    }

    public ImageSource LargeGlyph
    {
      get { return largeGlyph; }
    }

    public ICommand RunModuleCommand
    {
      get
      {
        if (runModuleCommand == null)
        {
          runModuleCommand = new Smv.MVVM.Commands.DelegateCommand(ExecRunModuleCommand);
        }
        return runModuleCommand;
      }
    }

    private void ExecRunModuleCommand()
    {
      EventHandler<Smv.RibbonUserUI.RibbonUIEventArgs> temp = RunEvent;
      if (temp != null)
        temp(this, new Smv.RibbonUserUI.RibbonUIEventArgs(new ViewMapDefects()));
    }
    
    public string CaptionControl
    {
      get { return "Карта деф-тов"; }
    }

    public string HintControl
    {
      get { return "Карта распределения дефектов на поверхности рулона"; }
    }

    public string NameControl
    {
      get { return "BtnMapDefects"; }
    }

    public Window MainWindow
    {
      get;
      set;
    }

    public Object CmParam { get; set; } 

    public MagLabContract()
    {
      largeGlyph = new BitmapImage(new Uri("pack://application:,,,/Viz.WrkModule.MapDefects;Component/Images/ModuleGlyph-32x32.ico"));
    }

  }

}