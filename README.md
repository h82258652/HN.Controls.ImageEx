﻿# HN.Controls.ImageEx
[![Build status](https://github.com/h82258652/HN.Controls.ImageEx/workflows/CI/badge.svg)](https://github.com/h82258652/HN.Controls.ImageEx/workflows/CI/badge.svg)

| Package | Nuget |
| - | - |
| HN.Controls.ImageEx.Core | [![Nuget](https://img.shields.io/nuget/v/HN.Controls.ImageEx.Core.svg)](https://www.nuget.org/packages/HN.Controls.ImageEx.Core) |
| HN.Controls.ImageEx.Wpf | [![Nuget](https://img.shields.io/nuget/v/HN.Controls.ImageEx.Wpf.svg)](https://www.nuget.org/packages/HN.Controls.ImageEx.Wpf) |
| HN.Controls.ImageEx.Uwp | [![Nuget](https://img.shields.io/nuget/v/HN.Controls.ImageEx.Uwp.svg)](https://www.nuget.org/packages/HN.Controls.ImageEx.Uwp) |

A cached Image control for WPF and UWP.  
WPF version for .net framework 4.6.1  
UWP version for 17763  
But both you can downgrade, the source code is here.😀

Usage please see the Demo project.  

### Next plan: 
- [x] Update the lazy loading, support set the thresholds and give a default value.  
- [ ] Use ```SkiaSharp``` to resolve the image.  
- [ ] Target .net 5.  
- [ ] Built-in shadow, same API for WPF and UWP.  
- [ ] ```CornerRadius``` support.  
- [ ] Built-in fade in animation while loaded image success.  
- [ ] Same API for WPF and UWP ,such as ```StretchDirection```(exist in WPF but not UWP), ```NineGrid```(exist in UWP but not WPF).  
- [ ] Add ```IImageEx``` interface for ```ImageEx```.  