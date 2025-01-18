; ModuleID = 'marshal_methods.armeabi-v7a.ll'
source_filename = "marshal_methods.armeabi-v7a.ll"
target datalayout = "e-m:e-p:32:32-Fi8-i64:64-v128:64:128-a:0:32-n32-S64"
target triple = "armv7-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [349 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [692 x i32] [
	i32 2616222, ; 0: System.Net.NetworkInformation.dll => 0x27eb9e => 69
	i32 10166715, ; 1: System.Net.NameResolution.dll => 0x9b21bb => 68
	i32 15721112, ; 2: System.Runtime.Intrinsics.dll => 0xefe298 => 109
	i32 26230656, ; 3: Microsoft.Extensions.DependencyModel => 0x1903f80 => 198
	i32 32687329, ; 4: Xamarin.AndroidX.Lifecycle.Runtime => 0x1f2c4e1 => 268
	i32 34715100, ; 5: Xamarin.Google.Guava.ListenableFuture.dll => 0x211b5dc => 302
	i32 34839235, ; 6: System.IO.FileSystem.DriveInfo => 0x2139ac3 => 49
	i32 39109920, ; 7: Newtonsoft.Json.dll => 0x254c520 => 211
	i32 39485524, ; 8: System.Net.WebSockets.dll => 0x25a8054 => 81
	i32 42639949, ; 9: System.Threading.Thread => 0x28aa24d => 146
	i32 52239042, ; 10: HtmlAgilityPack => 0x31d1ac2 => 179
	i32 65960268, ; 11: Microsoft.Win32.SystemEvents.dll => 0x3ee794c => 210
	i32 66541672, ; 12: System.Diagnostics.StackTrace => 0x3f75868 => 31
	i32 67008169, ; 13: zh-Hant\Microsoft.Maui.Controls.resources => 0x3fe76a9 => 343
	i32 68219467, ; 14: System.Security.Cryptography.Primitives => 0x410f24b => 125
	i32 72070932, ; 15: Microsoft.Maui.Graphics.dll => 0x44bb714 => 209
	i32 82292897, ; 16: System.Runtime.CompilerServices.VisualC.dll => 0x4e7b0a1 => 103
	i32 101534019, ; 17: Xamarin.AndroidX.SlidingPaneLayout => 0x60d4943 => 286
	i32 117431740, ; 18: System.Runtime.InteropServices => 0x6ffddbc => 108
	i32 120558881, ; 19: Xamarin.AndroidX.SlidingPaneLayout.dll => 0x72f9521 => 286
	i32 122350210, ; 20: System.Threading.Channels.dll => 0x74aea82 => 140
	i32 134690465, ; 21: Xamarin.Kotlin.StdLib.Jdk7.dll => 0x80736a1 => 306
	i32 142721839, ; 22: System.Net.WebHeaderCollection => 0x881c32f => 78
	i32 149972175, ; 23: System.Security.Cryptography.Primitives.dll => 0x8f064cf => 125
	i32 159306688, ; 24: System.ComponentModel.Annotations => 0x97ed3c0 => 14
	i32 165246403, ; 25: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 242
	i32 176265551, ; 26: System.ServiceProcess => 0xa81994f => 133
	i32 182336117, ; 27: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 288
	i32 184328833, ; 28: System.ValueTuple.dll => 0xafca281 => 152
	i32 195452805, ; 29: vi/Microsoft.Maui.Controls.resources.dll => 0xba65f85 => 340
	i32 199333315, ; 30: zh-HK/Microsoft.Maui.Controls.resources.dll => 0xbe195c3 => 341
	i32 205061960, ; 31: System.ComponentModel => 0xc38ff48 => 19
	i32 209399409, ; 32: Xamarin.AndroidX.Browser.dll => 0xc7b2e71 => 240
	i32 220171995, ; 33: System.Diagnostics.Debug => 0xd1f8edb => 27
	i32 230216969, ; 34: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0xdb8d509 => 262
	i32 230752869, ; 35: Microsoft.CSharp.dll => 0xdc10265 => 2
	i32 231409092, ; 36: System.Linq.Parallel => 0xdcb05c4 => 60
	i32 231814094, ; 37: System.Globalization => 0xdd133ce => 43
	i32 246610117, ; 38: System.Reflection.Emit.Lightweight => 0xeb2f8c5 => 92
	i32 261689757, ; 39: Xamarin.AndroidX.ConstraintLayout.dll => 0xf99119d => 245
	i32 276479776, ; 40: System.Threading.Timer.dll => 0x107abf20 => 148
	i32 278686392, ; 41: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x109c6ab8 => 264
	i32 280482487, ; 42: Xamarin.AndroidX.Interpolator => 0x10b7d2b7 => 261
	i32 280992041, ; 43: cs/Microsoft.Maui.Controls.resources.dll => 0x10bf9929 => 312
	i32 291076382, ; 44: System.IO.Pipes.AccessControl.dll => 0x1159791e => 55
	i32 298918909, ; 45: System.Net.Ping.dll => 0x11d123fd => 70
	i32 317674968, ; 46: vi\Microsoft.Maui.Controls.resources => 0x12ef55d8 => 340
	i32 318968648, ; 47: Xamarin.AndroidX.Activity.dll => 0x13031348 => 231
	i32 321597661, ; 48: System.Numerics => 0x132b30dd => 84
	i32 336156722, ; 49: ja/Microsoft.Maui.Controls.resources.dll => 0x14095832 => 325
	i32 342366114, ; 50: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 263
	i32 356389973, ; 51: it/Microsoft.Maui.Controls.resources.dll => 0x153e1455 => 324
	i32 360082299, ; 52: System.ServiceModel.Web => 0x15766b7b => 132
	i32 367780167, ; 53: System.IO.Pipes => 0x15ebe147 => 56
	i32 374914964, ; 54: System.Transactions.Local => 0x1658bf94 => 150
	i32 375677976, ; 55: System.Net.ServicePoint.dll => 0x16646418 => 75
	i32 379916513, ; 56: System.Threading.Thread.dll => 0x16a510e1 => 146
	i32 385762202, ; 57: System.Memory.dll => 0x16fe439a => 63
	i32 392610295, ; 58: System.Threading.ThreadPool.dll => 0x1766c1f7 => 147
	i32 393699800, ; 59: Firebase => 0x177761d8 => 175
	i32 395744057, ; 60: _Microsoft.Android.Resource.Designer => 0x17969339 => 345
	i32 403441872, ; 61: WindowsBase => 0x180c08d0 => 166
	i32 435591531, ; 62: sv/Microsoft.Maui.Controls.resources.dll => 0x19f6996b => 336
	i32 441335492, ; 63: Xamarin.AndroidX.ConstraintLayout.Core => 0x1a4e3ec4 => 246
	i32 442565967, ; 64: System.Collections => 0x1a61054f => 13
	i32 450948140, ; 65: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 259
	i32 451504562, ; 66: System.Security.Cryptography.X509Certificates => 0x1ae969b2 => 126
	i32 456227837, ; 67: System.Web.HttpUtility.dll => 0x1b317bfd => 153
	i32 459347974, ; 68: System.Runtime.Serialization.Primitives.dll => 0x1b611806 => 114
	i32 465846621, ; 69: mscorlib => 0x1bc4415d => 167
	i32 469710990, ; 70: System.dll => 0x1bff388e => 165
	i32 476646585, ; 71: Xamarin.AndroidX.Interpolator.dll => 0x1c690cb9 => 261
	i32 486930444, ; 72: Xamarin.AndroidX.LocalBroadcastManager.dll => 0x1d05f80c => 274
	i32 498788369, ; 73: System.ObjectModel => 0x1dbae811 => 85
	i32 500358224, ; 74: id/Microsoft.Maui.Controls.resources.dll => 0x1dd2dc50 => 323
	i32 503918385, ; 75: fi/Microsoft.Maui.Controls.resources.dll => 0x1e092f31 => 317
	i32 513247710, ; 76: Microsoft.Extensions.Primitives.dll => 0x1e9789de => 203
	i32 525008092, ; 77: SkiaSharp.dll => 0x1f4afcdc => 214
	i32 526420162, ; 78: System.Transactions.dll => 0x1f6088c2 => 151
	i32 527452488, ; 79: Xamarin.Kotlin.StdLib.Jdk7 => 0x1f704948 => 306
	i32 530272170, ; 80: System.Linq.Queryable => 0x1f9b4faa => 61
	i32 539058512, ; 81: Microsoft.Extensions.Logging => 0x20216150 => 199
	i32 540030774, ; 82: System.IO.FileSystem.dll => 0x20303736 => 52
	i32 545304856, ; 83: System.Runtime.Extensions => 0x2080b118 => 104
	i32 546455878, ; 84: System.Runtime.Serialization.Xml => 0x20924146 => 115
	i32 549171840, ; 85: System.Globalization.Calendars => 0x20bbb280 => 41
	i32 557405415, ; 86: Jsr305Binding => 0x213954e7 => 299
	i32 569601784, ; 87: Xamarin.AndroidX.Window.Extensions.Core.Core => 0x21f36ef8 => 297
	i32 577335427, ; 88: System.Security.Cryptography.Cng => 0x22697083 => 121
	i32 582345703, ; 89: PDFiumSharp.dll => 0x22b5e3e7 => 212
	i32 592146354, ; 90: pt-BR/Microsoft.Maui.Controls.resources.dll => 0x234b6fb2 => 331
	i32 601371474, ; 91: System.IO.IsolatedStorage.dll => 0x23d83352 => 53
	i32 605376203, ; 92: System.IO.Compression.FileSystem => 0x24154ecb => 45
	i32 606421715, ; 93: itext.layout => 0x242542d3 => 185
	i32 610194910, ; 94: System.Reactive.dll => 0x245ed5de => 225
	i32 613668793, ; 95: System.Security.Cryptography.Algorithms => 0x2493d7b9 => 120
	i32 627609679, ; 96: Xamarin.AndroidX.CustomView => 0x2568904f => 251
	i32 627931235, ; 97: nl\Microsoft.Maui.Controls.resources => 0x256d7863 => 329
	i32 639843206, ; 98: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 0x26233b86 => 257
	i32 643868501, ; 99: System.Net => 0x2660a755 => 82
	i32 662205335, ; 100: System.Text.Encodings.Web.dll => 0x27787397 => 137
	i32 663517072, ; 101: Xamarin.AndroidX.VersionedParcelable => 0x278c7790 => 293
	i32 666292255, ; 102: Xamarin.AndroidX.Arch.Core.Common.dll => 0x27b6d01f => 238
	i32 672442732, ; 103: System.Collections.Concurrent => 0x2814a96c => 9
	i32 683518922, ; 104: System.Net.Security => 0x28bdabca => 74
	i32 688181140, ; 105: ca/Microsoft.Maui.Controls.resources.dll => 0x2904cf94 => 311
	i32 690569205, ; 106: System.Xml.Linq.dll => 0x29293ff5 => 156
	i32 691348768, ; 107: Xamarin.KotlinX.Coroutines.Android.dll => 0x29352520 => 308
	i32 693804605, ; 108: System.Windows => 0x295a9e3d => 155
	i32 699345723, ; 109: System.Reflection.Emit => 0x29af2b3b => 93
	i32 700284507, ; 110: Xamarin.Jetbrains.Annotations => 0x29bd7e5b => 303
	i32 700358131, ; 111: System.IO.Compression.ZipFile => 0x29be9df3 => 46
	i32 706645707, ; 112: ko/Microsoft.Maui.Controls.resources.dll => 0x2a1e8ecb => 326
	i32 709557578, ; 113: de/Microsoft.Maui.Controls.resources.dll => 0x2a4afd4a => 314
	i32 720511267, ; 114: Xamarin.Kotlin.StdLib.Jdk8 => 0x2af22123 => 307
	i32 722857257, ; 115: System.Runtime.Loader.dll => 0x2b15ed29 => 110
	i32 735137430, ; 116: System.Security.SecureString.dll => 0x2bd14e96 => 130
	i32 752232764, ; 117: System.Diagnostics.Contracts.dll => 0x2cd6293c => 26
	i32 755313932, ; 118: Xamarin.Android.Glide.Annotations.dll => 0x2d052d0c => 228
	i32 759454413, ; 119: System.Net.Requests => 0x2d445acd => 73
	i32 762598435, ; 120: System.IO.Pipes.dll => 0x2d745423 => 56
	i32 767702248, ; 121: nl\PdfiumViewer.resources => 0x2dc234e8 => 344
	i32 775507847, ; 122: System.IO.Compression => 0x2e394f87 => 47
	i32 777317022, ; 123: sk\Microsoft.Maui.Controls.resources => 0x2e54ea9e => 335
	i32 778756650, ; 124: SkiaSharp.HarfBuzz.dll => 0x2e6ae22a => 215
	i32 789151979, ; 125: Microsoft.Extensions.Options => 0x2f0980eb => 202
	i32 790371945, ; 126: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 0x2f1c1e69 => 252
	i32 804715423, ; 127: System.Data.Common => 0x2ff6fb9f => 23
	i32 807930345, ; 128: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 0x302809e9 => 266
	i32 809851609, ; 129: System.Drawing.Common.dll => 0x30455ad9 => 224
	i32 823281589, ; 130: System.Private.Uri.dll => 0x311247b5 => 87
	i32 830298997, ; 131: System.IO.Compression.Brotli => 0x317d5b75 => 44
	i32 832635846, ; 132: System.Xml.XPath.dll => 0x31a103c6 => 161
	i32 834051424, ; 133: System.Net.Quic => 0x31b69d60 => 72
	i32 843511501, ; 134: Xamarin.AndroidX.Print => 0x3246f6cd => 279
	i32 873119928, ; 135: Microsoft.VisualBasic => 0x340ac0b8 => 4
	i32 877678880, ; 136: System.Globalization.dll => 0x34505120 => 43
	i32 878954865, ; 137: System.Net.Http.Json => 0x3463c971 => 64
	i32 904024072, ; 138: System.ComponentModel.Primitives.dll => 0x35e25008 => 17
	i32 911108515, ; 139: System.IO.MemoryMappedFiles.dll => 0x364e69a3 => 54
	i32 917108320, ; 140: itext.io => 0x36a9f660 => 183
	i32 926902833, ; 141: tr/Microsoft.Maui.Controls.resources.dll => 0x373f6a31 => 338
	i32 928116545, ; 142: Xamarin.Google.Guava.ListenableFuture => 0x3751ef41 => 302
	i32 952186615, ; 143: System.Runtime.InteropServices.JavaScript.dll => 0x38c136f7 => 106
	i32 955402788, ; 144: Newtonsoft.Json => 0x38f24a24 => 211
	i32 956575887, ; 145: Xamarin.Kotlin.StdLib.Jdk8.dll => 0x3904308f => 307
	i32 966729478, ; 146: Xamarin.Google.Crypto.Tink.Android => 0x399f1f06 => 300
	i32 967690846, ; 147: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 263
	i32 971744099, ; 148: itext.barcodes.dll => 0x39eba363 => 180
	i32 975236339, ; 149: System.Diagnostics.Tracing => 0x3a20ecf3 => 35
	i32 975874589, ; 150: System.Xml.XDocument => 0x3a2aaa1d => 159
	i32 986514023, ; 151: System.Private.DataContractSerialization.dll => 0x3acd0267 => 86
	i32 987214855, ; 152: System.Diagnostics.Tools => 0x3ad7b407 => 33
	i32 992768348, ; 153: System.Collections.dll => 0x3b2c715c => 13
	i32 994442037, ; 154: System.IO.FileSystem => 0x3b45fb35 => 52
	i32 1001831731, ; 155: System.IO.UnmanagedMemoryStream.dll => 0x3bb6bd33 => 57
	i32 1008871213, ; 156: VersOne.Epub => 0x3c22272d => 226
	i32 1012816738, ; 157: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 283
	i32 1019214401, ; 158: System.Drawing => 0x3cbffa41 => 37
	i32 1028951442, ; 159: Microsoft.Extensions.DependencyInjection.Abstractions => 0x3d548d92 => 197
	i32 1029334545, ; 160: da/Microsoft.Maui.Controls.resources.dll => 0x3d5a6611 => 313
	i32 1031528504, ; 161: Xamarin.Google.ErrorProne.Annotations.dll => 0x3d7be038 => 301
	i32 1035644815, ; 162: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 236
	i32 1036536393, ; 163: System.Drawing.Primitives.dll => 0x3dc84a49 => 36
	i32 1044663988, ; 164: System.Linq.Expressions.dll => 0x3e444eb4 => 59
	i32 1052210849, ; 165: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 270
	i32 1067306892, ; 166: GoogleGson => 0x3f9dcf8c => 177
	i32 1067609627, ; 167: itext.forms => 0x3fa26e1b => 182
	i32 1074246011, ; 168: itext.kernel.dll => 0x4007b17b => 184
	i32 1082857460, ; 169: System.ComponentModel.TypeConverter => 0x408b17f4 => 18
	i32 1084122840, ; 170: Xamarin.Kotlin.StdLib => 0x409e66d8 => 304
	i32 1098259244, ; 171: System => 0x41761b2c => 165
	i32 1099692271, ; 172: Microsoft.DotNet.PlatformAbstractions => 0x418bf8ef => 193
	i32 1118262833, ; 173: ko\Microsoft.Maui.Controls.resources => 0x42a75631 => 326
	i32 1121599056, ; 174: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 0x42da3e50 => 269
	i32 1127624469, ; 175: Microsoft.Extensions.Logging.Debug => 0x43362f15 => 201
	i32 1149092582, ; 176: Xamarin.AndroidX.Window => 0x447dc2e6 => 296
	i32 1168523401, ; 177: pt\Microsoft.Maui.Controls.resources => 0x45a64089 => 332
	i32 1170634674, ; 178: System.Web.dll => 0x45c677b2 => 154
	i32 1175144683, ; 179: Xamarin.AndroidX.VectorDrawable.Animated => 0x460b48eb => 292
	i32 1178241025, ; 180: Xamarin.AndroidX.Navigation.Runtime.dll => 0x463a8801 => 277
	i32 1203215381, ; 181: pl/Microsoft.Maui.Controls.resources.dll => 0x47b79c15 => 330
	i32 1204270330, ; 182: Xamarin.AndroidX.Arch.Core.Common => 0x47c7b4fa => 238
	i32 1208641965, ; 183: System.Diagnostics.Process => 0x480a69ad => 30
	i32 1219128291, ; 184: System.IO.IsolatedStorage => 0x48aa6be3 => 53
	i32 1222247595, ; 185: itext.pdfua.dll => 0x48da04ab => 187
	i32 1234928153, ; 186: nb/Microsoft.Maui.Controls.resources.dll => 0x499b8219 => 328
	i32 1243150071, ; 187: Xamarin.AndroidX.Window.Extensions.Core.Core.dll => 0x4a18f6f7 => 297
	i32 1245460359, ; 188: itext.svg => 0x4a3c3787 => 190
	i32 1250430400, ; 189: itext.commons.dll => 0x4a880dc0 => 191
	i32 1253011324, ; 190: Microsoft.Win32.Registry => 0x4aaf6f7c => 6
	i32 1260983243, ; 191: cs\Microsoft.Maui.Controls.resources => 0x4b2913cb => 312
	i32 1264511973, ; 192: Xamarin.AndroidX.Startup.StartupRuntime.dll => 0x4b5eebe5 => 287
	i32 1264538220, ; 193: Syncfusion.Compression.Portable => 0x4b5f526c => 219
	i32 1267360935, ; 194: Xamarin.AndroidX.VectorDrawable => 0x4b8a64a7 => 291
	i32 1273260888, ; 195: Xamarin.AndroidX.Collection.Ktx => 0x4be46b58 => 243
	i32 1275534314, ; 196: Xamarin.KotlinX.Coroutines.Android => 0x4c071bea => 308
	i32 1278448581, ; 197: Xamarin.AndroidX.Annotation.Jvm => 0x4c3393c5 => 235
	i32 1278779541, ; 198: itext.pdfa.dll => 0x4c38a095 => 186
	i32 1293217323, ; 199: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 254
	i32 1299589805, ; 200: Syncfusion.SkiaSharpHelper.Portable => 0x4d762aad => 223
	i32 1309188875, ; 201: System.Private.DataContractSerialization => 0x4e08a30b => 86
	i32 1322716291, ; 202: Xamarin.AndroidX.Window.dll => 0x4ed70c83 => 296
	i32 1324164729, ; 203: System.Linq => 0x4eed2679 => 62
	i32 1335329327, ; 204: System.Runtime.Serialization.Json.dll => 0x4f97822f => 113
	i32 1364015309, ; 205: System.IO => 0x514d38cd => 58
	i32 1373134921, ; 206: zh-Hans\Microsoft.Maui.Controls.resources => 0x51d86049 => 342
	i32 1376866003, ; 207: Xamarin.AndroidX.SavedState => 0x52114ed3 => 283
	i32 1379779777, ; 208: System.Resources.ResourceManager => 0x523dc4c1 => 100
	i32 1402170036, ; 209: System.Configuration.dll => 0x53936ab4 => 20
	i32 1406073936, ; 210: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 247
	i32 1408764838, ; 211: System.Runtime.Serialization.Formatters.dll => 0x53f80ba6 => 112
	i32 1411638395, ; 212: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 102
	i32 1422545099, ; 213: System.Runtime.CompilerServices.VisualC => 0x54ca50cb => 103
	i32 1430672901, ; 214: ar\Microsoft.Maui.Controls.resources => 0x55465605 => 310
	i32 1434145427, ; 215: System.Runtime.Handles => 0x557b5293 => 105
	i32 1435222561, ; 216: Xamarin.Google.Crypto.Tink.Android.dll => 0x558bc221 => 300
	i32 1439761251, ; 217: System.Net.Quic.dll => 0x55d10363 => 72
	i32 1452070440, ; 218: System.Formats.Asn1.dll => 0x568cd628 => 39
	i32 1453312822, ; 219: System.Diagnostics.Tools.dll => 0x569fcb36 => 33
	i32 1457743152, ; 220: System.Runtime.Extensions.dll => 0x56e36530 => 104
	i32 1458022317, ; 221: System.Net.Security.dll => 0x56e7a7ad => 74
	i32 1461004990, ; 222: es\Microsoft.Maui.Controls.resources => 0x57152abe => 316
	i32 1461234159, ; 223: System.Collections.Immutable.dll => 0x5718a9ef => 10
	i32 1461719063, ; 224: System.Security.Cryptography.OpenSsl => 0x57201017 => 124
	i32 1462112819, ; 225: System.IO.Compression.dll => 0x57261233 => 47
	i32 1469204771, ; 226: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 237
	i32 1470490898, ; 227: Microsoft.Extensions.Primitives => 0x57a5e912 => 203
	i32 1479771757, ; 228: System.Collections.Immutable => 0x5833866d => 10
	i32 1480492111, ; 229: System.IO.Compression.Brotli.dll => 0x583e844f => 44
	i32 1487239319, ; 230: Microsoft.Win32.Primitives => 0x58a57897 => 5
	i32 1488664300, ; 231: itext.bouncy-castle-connector.dll => 0x58bb36ec => 181
	i32 1490025113, ; 232: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 0x58cffa99 => 284
	i32 1493001747, ; 233: hi/Microsoft.Maui.Controls.resources.dll => 0x58fd6613 => 320
	i32 1514721132, ; 234: el/Microsoft.Maui.Controls.resources.dll => 0x5a48cf6c => 315
	i32 1536373174, ; 235: System.Diagnostics.TextWriterTraceListener => 0x5b9331b6 => 32
	i32 1543031311, ; 236: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 139
	i32 1543355203, ; 237: System.Reflection.Emit.dll => 0x5bfdbb43 => 93
	i32 1550322496, ; 238: System.Reflection.Extensions.dll => 0x5c680b40 => 94
	i32 1551623176, ; 239: sk/Microsoft.Maui.Controls.resources.dll => 0x5c7be408 => 335
	i32 1565862583, ; 240: System.IO.FileSystem.Primitives => 0x5d552ab7 => 50
	i32 1566207040, ; 241: System.Threading.Tasks.Dataflow.dll => 0x5d5a6c40 => 142
	i32 1573704789, ; 242: System.Runtime.Serialization.Json => 0x5dccd455 => 113
	i32 1580037396, ; 243: System.Threading.Overlapped => 0x5e2d7514 => 141
	i32 1582372066, ; 244: Xamarin.AndroidX.DocumentFile.dll => 0x5e5114e2 => 253
	i32 1592978981, ; 245: System.Runtime.Serialization.dll => 0x5ef2ee25 => 116
	i32 1593874069, ; 246: AppProyectoFinal => 0x5f009695 => 1
	i32 1597949149, ; 247: Xamarin.Google.ErrorProne.Annotations => 0x5f3ec4dd => 301
	i32 1601112923, ; 248: System.Xml.Serialization => 0x5f6f0b5b => 158
	i32 1604827217, ; 249: System.Net.WebClient => 0x5fa7b851 => 77
	i32 1612216473, ; 250: PdfiumViewer.dll => 0x60187899 => 213
	i32 1618516317, ; 251: System.Net.WebSockets.Client.dll => 0x6078995d => 80
	i32 1622152042, ; 252: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 273
	i32 1622358360, ; 253: System.Dynamic.Runtime => 0x60b33958 => 38
	i32 1623212457, ; 254: SkiaSharp.Views.Maui.Controls => 0x60c041a9 => 217
	i32 1624863272, ; 255: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 295
	i32 1635184631, ; 256: Xamarin.AndroidX.Emoji2.ViewsHelper => 0x6176eff7 => 257
	i32 1636350590, ; 257: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 250
	i32 1639515021, ; 258: System.Net.Http.dll => 0x61b9038d => 65
	i32 1639986890, ; 259: System.Text.RegularExpressions => 0x61c036ca => 139
	i32 1641389582, ; 260: System.ComponentModel.EventBasedAsync.dll => 0x61d59e0e => 16
	i32 1657153582, ; 261: System.Runtime => 0x62c6282e => 117
	i32 1658241508, ; 262: Xamarin.AndroidX.Tracing.Tracing.dll => 0x62d6c1e4 => 289
	i32 1658251792, ; 263: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 298
	i32 1670060433, ; 264: Xamarin.AndroidX.ConstraintLayout => 0x638b1991 => 245
	i32 1672083785, ; 265: itext.pdfa => 0x63a9f949 => 186
	i32 1675553242, ; 266: System.IO.FileSystem.DriveInfo.dll => 0x63dee9da => 49
	i32 1677501392, ; 267: System.Net.Primitives.dll => 0x63fca3d0 => 71
	i32 1678508291, ; 268: System.Net.WebSockets => 0x640c0103 => 81
	i32 1679769178, ; 269: System.Security.Cryptography => 0x641f3e5a => 127
	i32 1691477237, ; 270: System.Reflection.Metadata => 0x64d1e4f5 => 95
	i32 1696967625, ; 271: System.Security.Cryptography.Csp => 0x6525abc9 => 122
	i32 1698840827, ; 272: Xamarin.Kotlin.StdLib.Common => 0x654240fb => 305
	i32 1699106640, ; 273: Syncfusion.PdfToImageConverter.Portable => 0x65464f50 => 222
	i32 1701541528, ; 274: System.Diagnostics.Debug.dll => 0x656b7698 => 27
	i32 1720223769, ; 275: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 0x66888819 => 266
	i32 1726116996, ; 276: System.Reflection.dll => 0x66e27484 => 98
	i32 1728033016, ; 277: System.Diagnostics.FileVersionInfo.dll => 0x66ffb0f8 => 29
	i32 1729485958, ; 278: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 241
	i32 1736233607, ; 279: ro/Microsoft.Maui.Controls.resources.dll => 0x677cd287 => 333
	i32 1743415430, ; 280: ca\Microsoft.Maui.Controls.resources => 0x67ea6886 => 311
	i32 1744735666, ; 281: System.Transactions.Local.dll => 0x67fe8db2 => 150
	i32 1746316138, ; 282: Mono.Android.Export => 0x6816ab6a => 170
	i32 1750313021, ; 283: Microsoft.Win32.Primitives.dll => 0x6853a83d => 5
	i32 1758240030, ; 284: System.Resources.Reader.dll => 0x68cc9d1e => 99
	i32 1763938596, ; 285: System.Diagnostics.TraceSource.dll => 0x69239124 => 34
	i32 1765942094, ; 286: System.Reflection.Extensions => 0x6942234e => 94
	i32 1766324549, ; 287: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 288
	i32 1770582343, ; 288: Microsoft.Extensions.Logging.dll => 0x6988f147 => 199
	i32 1776026572, ; 289: System.Core.dll => 0x69dc03cc => 22
	i32 1777075843, ; 290: System.Globalization.Extensions.dll => 0x69ec0683 => 42
	i32 1780572499, ; 291: Mono.Android.Runtime.dll => 0x6a216153 => 171
	i32 1782862114, ; 292: ms\Microsoft.Maui.Controls.resources => 0x6a445122 => 327
	i32 1788241197, ; 293: Xamarin.AndroidX.Fragment => 0x6a96652d => 259
	i32 1793755602, ; 294: he\Microsoft.Maui.Controls.resources => 0x6aea89d2 => 319
	i32 1808609942, ; 295: Xamarin.AndroidX.Loader => 0x6bcd3296 => 273
	i32 1813058853, ; 296: Xamarin.Kotlin.StdLib.dll => 0x6c111525 => 304
	i32 1813201214, ; 297: Xamarin.Google.Android.Material => 0x6c13413e => 298
	i32 1818569960, ; 298: Xamarin.AndroidX.Navigation.UI.dll => 0x6c652ce8 => 278
	i32 1818787751, ; 299: Microsoft.VisualBasic.Core => 0x6c687fa7 => 3
	i32 1824175904, ; 300: System.Text.Encoding.Extensions => 0x6cbab720 => 135
	i32 1824722060, ; 301: System.Runtime.Serialization.Formatters => 0x6cc30c8c => 112
	i32 1828688058, ; 302: Microsoft.Extensions.Logging.Abstractions.dll => 0x6cff90ba => 200
	i32 1842015223, ; 303: uk/Microsoft.Maui.Controls.resources.dll => 0x6dcaebf7 => 339
	i32 1847515442, ; 304: Xamarin.Android.Glide.Annotations => 0x6e1ed932 => 228
	i32 1853025655, ; 305: sv\Microsoft.Maui.Controls.resources => 0x6e72ed77 => 336
	i32 1858542181, ; 306: System.Linq.Expressions => 0x6ec71a65 => 59
	i32 1870277092, ; 307: System.Reflection.Primitives => 0x6f7a29e4 => 96
	i32 1875935024, ; 308: fr\Microsoft.Maui.Controls.resources => 0x6fd07f30 => 318
	i32 1879696579, ; 309: System.Formats.Tar.dll => 0x7009e4c3 => 40
	i32 1885316902, ; 310: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0x705fa726 => 239
	i32 1888955245, ; 311: System.Diagnostics.Contracts => 0x70972b6d => 26
	i32 1889954781, ; 312: System.Reflection.Metadata.dll => 0x70a66bdd => 95
	i32 1894524299, ; 313: Microsoft.DotNet.PlatformAbstractions.dll => 0x70ec258b => 193
	i32 1898237753, ; 314: System.Reflection.DispatchProxy => 0x7124cf39 => 90
	i32 1900610850, ; 315: System.Resources.ResourceManager.dll => 0x71490522 => 100
	i32 1910275211, ; 316: System.Collections.NonGeneric.dll => 0x71dc7c8b => 11
	i32 1939592360, ; 317: System.Private.Xml.Linq => 0x739bd4a8 => 88
	i32 1956758971, ; 318: System.Resources.Writer => 0x74a1c5bb => 101
	i32 1960744931, ; 319: Syncfusion.PdfToImageConverter.Portable.dll => 0x74de97e3 => 222
	i32 1961813231, ; 320: Xamarin.AndroidX.Security.SecurityCrypto.dll => 0x74eee4ef => 285
	i32 1968388702, ; 321: Microsoft.Extensions.Configuration.dll => 0x75533a5e => 194
	i32 1983156543, ; 322: Xamarin.Kotlin.StdLib.Common.dll => 0x7634913f => 305
	i32 1985761444, ; 323: Xamarin.Android.Glide.GifDecoder => 0x765c50a4 => 230
	i32 2003115576, ; 324: el\Microsoft.Maui.Controls.resources => 0x77651e38 => 315
	i32 2011961780, ; 325: System.Buffers.dll => 0x77ec19b4 => 8
	i32 2019465201, ; 326: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 270
	i32 2025202353, ; 327: ar/Microsoft.Maui.Controls.resources.dll => 0x78b622b1 => 310
	i32 2031763787, ; 328: Xamarin.Android.Glide => 0x791a414b => 227
	i32 2045470958, ; 329: System.Private.Xml => 0x79eb68ee => 89
	i32 2045845235, ; 330: itext.pdfua => 0x79f11ef3 => 187
	i32 2055257422, ; 331: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 265
	i32 2060060697, ; 332: System.Windows.dll => 0x7aca0819 => 155
	i32 2066184531, ; 333: de\Microsoft.Maui.Controls.resources => 0x7b277953 => 314
	i32 2070888862, ; 334: System.Diagnostics.TraceSource => 0x7b6f419e => 34
	i32 2079903147, ; 335: System.Runtime.dll => 0x7bf8cdab => 117
	i32 2090596640, ; 336: System.Numerics.Vectors => 0x7c9bf920 => 83
	i32 2127167465, ; 337: System.Console => 0x7ec9ffe9 => 21
	i32 2142473426, ; 338: System.Collections.Specialized => 0x7fb38cd2 => 12
	i32 2143790110, ; 339: System.Xml.XmlSerializer.dll => 0x7fc7a41e => 163
	i32 2146852085, ; 340: Microsoft.VisualBasic.dll => 0x7ff65cf5 => 4
	i32 2159891885, ; 341: Microsoft.Maui => 0x80bd55ad => 207
	i32 2169148018, ; 342: hu\Microsoft.Maui.Controls.resources => 0x814a9272 => 322
	i32 2181898931, ; 343: Microsoft.Extensions.Options.dll => 0x820d22b3 => 202
	i32 2192057212, ; 344: Microsoft.Extensions.Logging.Abstractions => 0x82a8237c => 200
	i32 2193016926, ; 345: System.ObjectModel.dll => 0x82b6c85e => 85
	i32 2197979891, ; 346: Microsoft.Extensions.DependencyModel.dll => 0x830282f3 => 198
	i32 2201107256, ; 347: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x83323b38 => 309
	i32 2201231467, ; 348: System.Net.Http => 0x8334206b => 65
	i32 2207618523, ; 349: it\Microsoft.Maui.Controls.resources => 0x839595db => 324
	i32 2216717168, ; 350: Firebase.Auth.dll => 0x84206b70 => 174
	i32 2217644978, ; 351: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x842e93b2 => 292
	i32 2222056684, ; 352: System.Threading.Tasks.Parallel => 0x8471e4ec => 144
	i32 2244775296, ; 353: Xamarin.AndroidX.LocalBroadcastManager => 0x85cc8d80 => 274
	i32 2252106437, ; 354: System.Xml.Serialization.dll => 0x863c6ac5 => 158
	i32 2256313426, ; 355: System.Globalization.Extensions => 0x867c9c52 => 42
	i32 2265110946, ; 356: System.Security.AccessControl.dll => 0x8702d9a2 => 118
	i32 2266799131, ; 357: Microsoft.Extensions.Configuration.Abstractions => 0x871c9c1b => 195
	i32 2267999099, ; 358: Xamarin.Android.Glide.DiskLruCache.dll => 0x872eeb7b => 229
	i32 2270573516, ; 359: fr/Microsoft.Maui.Controls.resources.dll => 0x875633cc => 318
	i32 2279755925, ; 360: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 281
	i32 2293034957, ; 361: System.ServiceModel.Web.dll => 0x88acefcd => 132
	i32 2295906218, ; 362: System.Net.Sockets => 0x88d8bfaa => 76
	i32 2298471582, ; 363: System.Net.Mail => 0x88ffe49e => 67
	i32 2303942373, ; 364: nb\Microsoft.Maui.Controls.resources => 0x89535ee5 => 328
	i32 2305521784, ; 365: System.Private.CoreLib.dll => 0x896b7878 => 173
	i32 2315684594, ; 366: Xamarin.AndroidX.Annotation.dll => 0x8a068af2 => 233
	i32 2320631194, ; 367: System.Threading.Tasks.Parallel.dll => 0x8a52059a => 144
	i32 2340441535, ; 368: System.Runtime.InteropServices.RuntimeInformation.dll => 0x8b804dbf => 107
	i32 2344264397, ; 369: System.ValueTuple => 0x8bbaa2cd => 152
	i32 2353062107, ; 370: System.Net.Primitives => 0x8c40e0db => 71
	i32 2354730003, ; 371: Syncfusion.Licensing => 0x8c5a5413 => 220
	i32 2364201794, ; 372: SkiaSharp.Views.Maui.Core => 0x8ceadb42 => 218
	i32 2368005991, ; 373: System.Xml.ReaderWriter.dll => 0x8d24e767 => 157
	i32 2371007202, ; 374: Microsoft.Extensions.Configuration => 0x8d52b2e2 => 194
	i32 2378619854, ; 375: System.Security.Cryptography.Csp.dll => 0x8dc6dbce => 122
	i32 2383496789, ; 376: System.Security.Principal.Windows.dll => 0x8e114655 => 128
	i32 2395872292, ; 377: id\Microsoft.Maui.Controls.resources => 0x8ece1c24 => 323
	i32 2401565422, ; 378: System.Web.HttpUtility => 0x8f24faee => 153
	i32 2403452196, ; 379: Xamarin.AndroidX.Emoji2.dll => 0x8f41c524 => 256
	i32 2421380589, ; 380: System.Threading.Tasks.Dataflow => 0x905355ed => 142
	i32 2423080555, ; 381: Xamarin.AndroidX.Collection.Ktx.dll => 0x906d466b => 243
	i32 2427813419, ; 382: hi\Microsoft.Maui.Controls.resources => 0x90b57e2b => 320
	i32 2431624606, ; 383: PdfiumViewer => 0x90efa59e => 213
	i32 2435356389, ; 384: System.Console.dll => 0x912896e5 => 21
	i32 2435904999, ; 385: System.ComponentModel.DataAnnotations.dll => 0x9130f5e7 => 15
	i32 2454642406, ; 386: System.Text.Encoding.dll => 0x924edee6 => 136
	i32 2458678730, ; 387: System.Net.Sockets.dll => 0x928c75ca => 76
	i32 2459001652, ; 388: System.Linq.Parallel.dll => 0x92916334 => 60
	i32 2465532216, ; 389: Xamarin.AndroidX.ConstraintLayout.Core.dll => 0x92f50938 => 246
	i32 2471841756, ; 390: netstandard.dll => 0x93554fdc => 168
	i32 2475788418, ; 391: Java.Interop.dll => 0x93918882 => 169
	i32 2480646305, ; 392: Microsoft.Maui.Controls => 0x93dba8a1 => 205
	i32 2483903535, ; 393: System.ComponentModel.EventBasedAsync => 0x940d5c2f => 16
	i32 2484371297, ; 394: System.Net.ServicePoint => 0x94147f61 => 75
	i32 2490993605, ; 395: System.AppContext.dll => 0x94798bc5 => 7
	i32 2501346920, ; 396: System.Data.DataSetExtensions => 0x95178668 => 24
	i32 2505896520, ; 397: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x955cf248 => 268
	i32 2522472828, ; 398: Xamarin.Android.Glide.dll => 0x9659e17c => 227
	i32 2538310050, ; 399: System.Reflection.Emit.Lightweight.dll => 0x974b89a2 => 92
	i32 2550873716, ; 400: hr\Microsoft.Maui.Controls.resources => 0x980b3e74 => 321
	i32 2562349572, ; 401: Microsoft.CSharp => 0x98ba5a04 => 2
	i32 2566497382, ; 402: itext.bouncy-castle-connector => 0x98f9a466 => 181
	i32 2570120770, ; 403: System.Text.Encodings.Web => 0x9930ee42 => 137
	i32 2573607077, ; 404: itext.kernel => 0x996620a5 => 184
	i32 2581783588, ; 405: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 0x99e2e424 => 269
	i32 2581819634, ; 406: Xamarin.AndroidX.VectorDrawable.dll => 0x99e370f2 => 291
	i32 2585220780, ; 407: System.Text.Encoding.Extensions.dll => 0x9a1756ac => 135
	i32 2585805581, ; 408: System.Net.Ping => 0x9a20430d => 70
	i32 2589602615, ; 409: System.Threading.ThreadPool => 0x9a5a3337 => 147
	i32 2593496499, ; 410: pl\Microsoft.Maui.Controls.resources => 0x9a959db3 => 330
	i32 2605712449, ; 411: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x9b500441 => 309
	i32 2615233544, ; 412: Xamarin.AndroidX.Fragment.Ktx => 0x9be14c08 => 260
	i32 2616218305, ; 413: Microsoft.Extensions.Logging.Debug.dll => 0x9bf052c1 => 201
	i32 2617129537, ; 414: System.Private.Xml.dll => 0x9bfe3a41 => 89
	i32 2618712057, ; 415: System.Reflection.TypeExtensions.dll => 0x9c165ff9 => 97
	i32 2620871830, ; 416: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 250
	i32 2624644809, ; 417: Xamarin.AndroidX.DynamicAnimation => 0x9c70e6c9 => 255
	i32 2625339995, ; 418: SkiaSharp.Views.Maui.Core.dll => 0x9c7b825b => 218
	i32 2626831493, ; 419: ja\Microsoft.Maui.Controls.resources => 0x9c924485 => 325
	i32 2627185994, ; 420: System.Diagnostics.TextWriterTraceListener.dll => 0x9c97ad4a => 32
	i32 2629843544, ; 421: System.IO.Compression.ZipFile.dll => 0x9cc03a58 => 46
	i32 2633051222, ; 422: Xamarin.AndroidX.Lifecycle.LiveData => 0x9cf12c56 => 264
	i32 2647286323, ; 423: en\AppProyectoFinal.resources => 0x9dca6233 => 0
	i32 2663391936, ; 424: Xamarin.Android.Glide.DiskLruCache => 0x9ec022c0 => 229
	i32 2663698177, ; 425: System.Runtime.Loader => 0x9ec4cf01 => 110
	i32 2664396074, ; 426: System.Xml.XDocument.dll => 0x9ecf752a => 159
	i32 2665622720, ; 427: System.Drawing.Primitives => 0x9ee22cc0 => 36
	i32 2676780864, ; 428: System.Data.Common.dll => 0x9f8c6f40 => 23
	i32 2686887180, ; 429: System.Runtime.Serialization.Xml.dll => 0xa026a50c => 115
	i32 2693849962, ; 430: System.IO.dll => 0xa090e36a => 58
	i32 2701096212, ; 431: Xamarin.AndroidX.Tracing.Tracing => 0xa0ff7514 => 289
	i32 2715334215, ; 432: System.Threading.Tasks.dll => 0xa1d8b647 => 145
	i32 2717744543, ; 433: System.Security.Claims => 0xa1fd7d9f => 119
	i32 2719963679, ; 434: System.Security.Cryptography.Cng.dll => 0xa21f5a1f => 121
	i32 2724373263, ; 435: System.Runtime.Numerics.dll => 0xa262a30f => 111
	i32 2732626843, ; 436: Xamarin.AndroidX.Activity => 0xa2e0939b => 231
	i32 2735172069, ; 437: System.Threading.Channels => 0xa30769e5 => 140
	i32 2737747696, ; 438: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 237
	i32 2740948882, ; 439: System.IO.Pipes.AccessControl => 0xa35f8f92 => 55
	i32 2748088231, ; 440: System.Runtime.InteropServices.JavaScript => 0xa3cc7fa7 => 106
	i32 2752995522, ; 441: pt-BR\Microsoft.Maui.Controls.resources => 0xa41760c2 => 331
	i32 2758225723, ; 442: Microsoft.Maui.Controls.Xaml => 0xa4672f3b => 206
	i32 2764765095, ; 443: Microsoft.Maui.dll => 0xa4caf7a7 => 207
	i32 2765824710, ; 444: System.Text.Encoding.CodePages.dll => 0xa4db22c6 => 134
	i32 2770495804, ; 445: Xamarin.Jetbrains.Annotations.dll => 0xa522693c => 303
	i32 2778768386, ; 446: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 294
	i32 2779977773, ; 447: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 0xa5b3182d => 282
	i32 2785988530, ; 448: th\Microsoft.Maui.Controls.resources => 0xa60ecfb2 => 337
	i32 2788224221, ; 449: Xamarin.AndroidX.Fragment.Ktx.dll => 0xa630ecdd => 260
	i32 2795602088, ; 450: SkiaSharp.Views.Android.dll => 0xa6a180a8 => 216
	i32 2795666278, ; 451: Microsoft.Win32.SystemEvents => 0xa6a27b66 => 210
	i32 2801831435, ; 452: Microsoft.Maui.Graphics => 0xa7008e0b => 209
	i32 2803228030, ; 453: System.Xml.XPath.XDocument.dll => 0xa715dd7e => 160
	i32 2806116107, ; 454: es/Microsoft.Maui.Controls.resources.dll => 0xa741ef0b => 316
	i32 2810250172, ; 455: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 247
	i32 2819470561, ; 456: System.Xml.dll => 0xa80db4e1 => 164
	i32 2821205001, ; 457: System.ServiceProcess.dll => 0xa8282c09 => 133
	i32 2821294376, ; 458: Xamarin.AndroidX.ResourceInspection.Annotation => 0xa8298928 => 282
	i32 2824502124, ; 459: System.Xml.XmlDocument => 0xa85a7b6c => 162
	i32 2831556043, ; 460: nl/Microsoft.Maui.Controls.resources.dll => 0xa8c61dcb => 329
	i32 2838993487, ; 461: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 0xa9379a4f => 271
	i32 2849599387, ; 462: System.Threading.Overlapped.dll => 0xa9d96f9b => 141
	i32 2853208004, ; 463: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 294
	i32 2855708567, ; 464: Xamarin.AndroidX.Transition => 0xaa36a797 => 290
	i32 2861098320, ; 465: Mono.Android.Export.dll => 0xaa88e550 => 170
	i32 2861189240, ; 466: Microsoft.Maui.Essentials => 0xaa8a4878 => 208
	i32 2868557005, ; 467: Syncfusion.Licensing.dll => 0xaafab4cd => 220
	i32 2870099610, ; 468: Xamarin.AndroidX.Activity.Ktx.dll => 0xab123e9a => 232
	i32 2875164099, ; 469: Jsr305Binding.dll => 0xab5f85c3 => 299
	i32 2875220617, ; 470: System.Globalization.Calendars.dll => 0xab606289 => 41
	i32 2884993177, ; 471: Xamarin.AndroidX.ExifInterface => 0xabf58099 => 258
	i32 2887636118, ; 472: System.Net.dll => 0xac1dd496 => 82
	i32 2899753641, ; 473: System.IO.UnmanagedMemoryStream => 0xacd6baa9 => 57
	i32 2900621748, ; 474: System.Dynamic.Runtime.dll => 0xace3f9b4 => 38
	i32 2901442782, ; 475: System.Reflection => 0xacf080de => 98
	i32 2905242038, ; 476: mscorlib.dll => 0xad2a79b6 => 167
	i32 2908639175, ; 477: itext.sign => 0xad5e4fc7 => 188
	i32 2909740682, ; 478: System.Private.CoreLib => 0xad6f1e8a => 173
	i32 2912489636, ; 479: SkiaSharp.Views.Android => 0xad9910a4 => 216
	i32 2916838712, ; 480: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 295
	i32 2919462931, ; 481: System.Numerics.Vectors.dll => 0xae037813 => 83
	i32 2921128767, ; 482: Xamarin.AndroidX.Annotation.Experimental.dll => 0xae1ce33f => 234
	i32 2936416060, ; 483: System.Resources.Reader => 0xaf06273c => 99
	i32 2940926066, ; 484: System.Diagnostics.StackTrace.dll => 0xaf4af872 => 31
	i32 2942453041, ; 485: System.Xml.XPath.XDocument => 0xaf624531 => 160
	i32 2959614098, ; 486: System.ComponentModel.dll => 0xb0682092 => 19
	i32 2968338931, ; 487: System.Security.Principal.Windows => 0xb0ed41f3 => 128
	i32 2972252294, ; 488: System.Security.Cryptography.Algorithms.dll => 0xb128f886 => 120
	i32 2978675010, ; 489: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 254
	i32 2987532451, ; 490: Xamarin.AndroidX.Security.SecurityCrypto => 0xb21220a3 => 285
	i32 2996846495, ; 491: Xamarin.AndroidX.Lifecycle.Process.dll => 0xb2a03f9f => 267
	i32 3016983068, ; 492: Xamarin.AndroidX.Startup.StartupRuntime => 0xb3d3821c => 287
	i32 3023353419, ; 493: WindowsBase.dll => 0xb434b64b => 166
	i32 3024354802, ; 494: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xb443fdf2 => 262
	i32 3038032645, ; 495: _Microsoft.Android.Resource.Designer.dll => 0xb514b305 => 345
	i32 3056245963, ; 496: Xamarin.AndroidX.SavedState.SavedState.Ktx => 0xb62a9ccb => 284
	i32 3057625584, ; 497: Xamarin.AndroidX.Navigation.Common => 0xb63fa9f0 => 275
	i32 3059408633, ; 498: Mono.Android.Runtime => 0xb65adef9 => 171
	i32 3059793426, ; 499: System.ComponentModel.Primitives => 0xb660be12 => 17
	i32 3075834255, ; 500: System.Threading.Tasks => 0xb755818f => 145
	i32 3077302341, ; 501: hu/Microsoft.Maui.Controls.resources.dll => 0xb76be845 => 322
	i32 3090735792, ; 502: System.Security.Cryptography.X509Certificates.dll => 0xb838e2b0 => 126
	i32 3099732863, ; 503: System.Security.Claims.dll => 0xb8c22b7f => 119
	i32 3103600923, ; 504: System.Formats.Asn1 => 0xb8fd311b => 39
	i32 3111772706, ; 505: System.Runtime.Serialization => 0xb979e222 => 116
	i32 3121463068, ; 506: System.IO.FileSystem.AccessControl.dll => 0xba0dbf1c => 48
	i32 3124832203, ; 507: System.Threading.Tasks.Extensions => 0xba4127cb => 143
	i32 3132293585, ; 508: System.Security.AccessControl => 0xbab301d1 => 118
	i32 3146401616, ; 509: itext.styledxmlparser => 0xbb8a4750 => 189
	i32 3147165239, ; 510: System.Diagnostics.Tracing.dll => 0xbb95ee37 => 35
	i32 3148237826, ; 511: GoogleGson.dll => 0xbba64c02 => 177
	i32 3159123045, ; 512: System.Reflection.Primitives.dll => 0xbc4c6465 => 96
	i32 3160747431, ; 513: System.IO.MemoryMappedFiles => 0xbc652da7 => 54
	i32 3178803400, ; 514: Xamarin.AndroidX.Navigation.Fragment.dll => 0xbd78b0c8 => 276
	i32 3192346100, ; 515: System.Security.SecureString => 0xbe4755f4 => 130
	i32 3193059034, ; 516: Syncfusion.SkiaSharpHelper.Portable.dll => 0xbe5236da => 223
	i32 3193515020, ; 517: System.Web => 0xbe592c0c => 154
	i32 3204380047, ; 518: System.Data.dll => 0xbefef58f => 25
	i32 3209718065, ; 519: System.Xml.XmlDocument.dll => 0xbf506931 => 162
	i32 3211777861, ; 520: Xamarin.AndroidX.DocumentFile => 0xbf6fd745 => 253
	i32 3220365878, ; 521: System.Threading => 0xbff2e236 => 149
	i32 3226221578, ; 522: System.Runtime.Handles.dll => 0xc04c3c0a => 105
	i32 3249927476, ; 523: PDFiumSharp => 0xc1b5f534 => 212
	i32 3251039220, ; 524: System.Reflection.DispatchProxy.dll => 0xc1c6ebf4 => 90
	i32 3258312781, ; 525: Xamarin.AndroidX.CardView => 0xc235e84d => 241
	i32 3265493905, ; 526: System.Linq.Queryable.dll => 0xc2a37b91 => 61
	i32 3265893370, ; 527: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 143
	i32 3277815716, ; 528: System.Resources.Writer.dll => 0xc35f7fa4 => 101
	i32 3279906254, ; 529: Microsoft.Win32.Registry.dll => 0xc37f65ce => 6
	i32 3280506390, ; 530: System.ComponentModel.Annotations.dll => 0xc3888e16 => 14
	i32 3290767353, ; 531: System.Security.Cryptography.Encoding => 0xc4251ff9 => 123
	i32 3299363146, ; 532: System.Text.Encoding => 0xc4a8494a => 136
	i32 3303498502, ; 533: System.Diagnostics.FileVersionInfo => 0xc4e76306 => 29
	i32 3305363605, ; 534: fi\Microsoft.Maui.Controls.resources => 0xc503d895 => 317
	i32 3316684772, ; 535: System.Net.Requests.dll => 0xc5b097e4 => 73
	i32 3317135071, ; 536: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 251
	i32 3317144872, ; 537: System.Data => 0xc5b79d28 => 25
	i32 3322403133, ; 538: Firebase.dll => 0xc607d93d => 175
	i32 3340387945, ; 539: SkiaSharp => 0xc71a4669 => 214
	i32 3340431453, ; 540: Xamarin.AndroidX.Arch.Core.Runtime => 0xc71af05d => 239
	i32 3342793838, ; 541: itext.barcodes => 0xc73efc6e => 180
	i32 3345895724, ; 542: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 0xc76e512c => 280
	i32 3346324047, ; 543: Xamarin.AndroidX.Navigation.Runtime => 0xc774da4f => 277
	i32 3357674450, ; 544: ru\Microsoft.Maui.Controls.resources => 0xc8220bd2 => 334
	i32 3358260929, ; 545: System.Text.Json => 0xc82afec1 => 138
	i32 3362336904, ; 546: Xamarin.AndroidX.Activity.Ktx => 0xc8693088 => 232
	i32 3362522851, ; 547: Xamarin.AndroidX.Core => 0xc86c06e3 => 248
	i32 3366347497, ; 548: Java.Interop => 0xc8a662e9 => 169
	i32 3374999561, ; 549: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 281
	i32 3381016424, ; 550: da\Microsoft.Maui.Controls.resources => 0xc9863768 => 313
	i32 3395150330, ; 551: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 102
	i32 3403906625, ; 552: System.Security.Cryptography.OpenSsl.dll => 0xcae37e41 => 124
	i32 3405233483, ; 553: Xamarin.AndroidX.CustomView.PoolingContainer => 0xcaf7bd4b => 252
	i32 3408837634, ; 554: nl/PdfiumViewer.resources.dll => 0xcb2ebc02 => 344
	i32 3428513518, ; 555: Microsoft.Extensions.DependencyInjection.dll => 0xcc5af6ee => 196
	i32 3429136800, ; 556: System.Xml => 0xcc6479a0 => 164
	i32 3430777524, ; 557: netstandard => 0xcc7d82b4 => 168
	i32 3433996769, ; 558: Syncfusion.Pdf.Portable.dll => 0xccaea1e1 => 221
	i32 3441283291, ; 559: Xamarin.AndroidX.DynamicAnimation.dll => 0xcd1dd0db => 255
	i32 3445260447, ; 560: System.Formats.Tar => 0xcd5a809f => 40
	i32 3452344032, ; 561: Microsoft.Maui.Controls.Compatibility.dll => 0xcdc696e0 => 204
	i32 3463511458, ; 562: hr/Microsoft.Maui.Controls.resources.dll => 0xce70fda2 => 321
	i32 3471940407, ; 563: System.ComponentModel.TypeConverter.dll => 0xcef19b37 => 18
	i32 3473156932, ; 564: SkiaSharp.Views.Maui.Controls.dll => 0xcf042b44 => 217
	i32 3476120550, ; 565: Mono.Android => 0xcf3163e6 => 172
	i32 3479583265, ; 566: ru/Microsoft.Maui.Controls.resources.dll => 0xcf663a21 => 334
	i32 3484440000, ; 567: ro\Microsoft.Maui.Controls.resources => 0xcfb055c0 => 333
	i32 3485117614, ; 568: System.Text.Json.dll => 0xcfbaacae => 138
	i32 3486566296, ; 569: System.Transactions => 0xcfd0c798 => 151
	i32 3493954962, ; 570: Xamarin.AndroidX.Concurrent.Futures.dll => 0xd0418592 => 244
	i32 3509114376, ; 571: System.Xml.Linq => 0xd128d608 => 156
	i32 3515174580, ; 572: System.Security.dll => 0xd1854eb4 => 131
	i32 3530912306, ; 573: System.Configuration => 0xd2757232 => 20
	i32 3539954161, ; 574: System.Net.HttpListener => 0xd2ff69f1 => 66
	i32 3551972787, ; 575: Syncfusion.Compression.Portable.dll => 0xd3b6cdb3 => 219
	i32 3560100363, ; 576: System.Threading.Timer => 0xd432d20b => 148
	i32 3570554715, ; 577: System.IO.FileSystem.AccessControl => 0xd4d2575b => 48
	i32 3580758918, ; 578: zh-HK\Microsoft.Maui.Controls.resources => 0xd56e0b86 => 341
	i32 3596207933, ; 579: LiteDB.dll => 0xd659c73d => 192
	i32 3597029428, ; 580: Xamarin.Android.Glide.GifDecoder.dll => 0xd6665034 => 230
	i32 3598340787, ; 581: System.Net.WebSockets.Client => 0xd67a52b3 => 80
	i32 3608519521, ; 582: System.Linq.dll => 0xd715a361 => 62
	i32 3624195450, ; 583: System.Runtime.InteropServices.RuntimeInformation => 0xd804d57a => 107
	i32 3627220390, ; 584: Xamarin.AndroidX.Print.dll => 0xd832fda6 => 279
	i32 3629588173, ; 585: LiteDB => 0xd8571ecd => 192
	i32 3633644679, ; 586: Xamarin.AndroidX.Annotation.Experimental => 0xd8950487 => 234
	i32 3637786959, ; 587: itext.sign.dll => 0xd8d4394f => 188
	i32 3638274909, ; 588: System.IO.FileSystem.Primitives.dll => 0xd8dbab5d => 50
	i32 3641597786, ; 589: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 265
	i32 3643446276, ; 590: tr\Microsoft.Maui.Controls.resources => 0xd92a9404 => 338
	i32 3643854240, ; 591: Xamarin.AndroidX.Navigation.Fragment => 0xd930cda0 => 276
	i32 3645089577, ; 592: System.ComponentModel.DataAnnotations => 0xd943a729 => 15
	i32 3655481159, ; 593: Firebase.Storage => 0xd9e23747 => 176
	i32 3657292374, ; 594: Microsoft.Extensions.Configuration.Abstractions.dll => 0xd9fdda56 => 195
	i32 3660523487, ; 595: System.Net.NetworkInformation => 0xda2f27df => 69
	i32 3672681054, ; 596: Mono.Android.dll => 0xdae8aa5e => 172
	i32 3682565725, ; 597: Xamarin.AndroidX.Browser => 0xdb7f7e5d => 240
	i32 3684561358, ; 598: Xamarin.AndroidX.Concurrent.Futures => 0xdb9df1ce => 244
	i32 3684657769, ; 599: itext.commons => 0xdb9f6a69 => 191
	i32 3689375977, ; 600: System.Drawing.Common => 0xdbe768e9 => 224
	i32 3697841164, ; 601: zh-Hant/Microsoft.Maui.Controls.resources.dll => 0xdc68940c => 343
	i32 3700866549, ; 602: System.Net.WebProxy.dll => 0xdc96bdf5 => 79
	i32 3706696989, ; 603: Xamarin.AndroidX.Core.Core.Ktx.dll => 0xdcefb51d => 249
	i32 3716563718, ; 604: System.Runtime.Intrinsics => 0xdd864306 => 109
	i32 3718780102, ; 605: Xamarin.AndroidX.Annotation => 0xdda814c6 => 233
	i32 3724971120, ; 606: Xamarin.AndroidX.Navigation.Common.dll => 0xde068c70 => 275
	i32 3731644420, ; 607: System.Reactive => 0xde6c6004 => 225
	i32 3732100267, ; 608: System.Net.NameResolution => 0xde7354ab => 68
	i32 3737834244, ; 609: System.Net.Http.Json.dll => 0xdecad304 => 64
	i32 3748608112, ; 610: System.Diagnostics.DiagnosticSource => 0xdf6f3870 => 28
	i32 3751444290, ; 611: System.Xml.XPath => 0xdf9a7f42 => 161
	i32 3786282454, ; 612: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 242
	i32 3792276235, ; 613: System.Collections.NonGeneric => 0xe2098b0b => 11
	i32 3792835768, ; 614: HarfBuzzSharp => 0xe21214b8 => 178
	i32 3800979733, ; 615: Microsoft.Maui.Controls.Compatibility => 0xe28e5915 => 204
	i32 3802395368, ; 616: System.Collections.Specialized.dll => 0xe2a3f2e8 => 12
	i32 3810220126, ; 617: HtmlAgilityPack.dll => 0xe31b585e => 179
	i32 3819260425, ; 618: System.Net.WebProxy => 0xe3a54a09 => 79
	i32 3823082795, ; 619: System.Security.Cryptography.dll => 0xe3df9d2b => 127
	i32 3828876191, ; 620: AppProyectoFinal.dll => 0xe438039f => 1
	i32 3829621856, ; 621: System.Numerics.dll => 0xe4436460 => 84
	i32 3841636137, ; 622: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 0xe4fab729 => 197
	i32 3844307129, ; 623: System.Net.Mail.dll => 0xe52378b9 => 67
	i32 3849253459, ; 624: System.Runtime.InteropServices.dll => 0xe56ef253 => 108
	i32 3870376305, ; 625: System.Net.HttpListener.dll => 0xe6b14171 => 66
	i32 3873536506, ; 626: System.Security.Principal => 0xe6e179fa => 129
	i32 3875112723, ; 627: System.Security.Cryptography.Encoding.dll => 0xe6f98713 => 123
	i32 3885497537, ; 628: System.Net.WebHeaderCollection.dll => 0xe797fcc1 => 78
	i32 3885922214, ; 629: Xamarin.AndroidX.Transition.dll => 0xe79e77a6 => 290
	i32 3888767677, ; 630: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 0xe7c9e2bd => 280
	i32 3889960447, ; 631: zh-Hans/Microsoft.Maui.Controls.resources.dll => 0xe7dc15ff => 342
	i32 3896106733, ; 632: System.Collections.Concurrent.dll => 0xe839deed => 9
	i32 3896760992, ; 633: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 248
	i32 3898862222, ; 634: itext.layout.dll => 0xe863ea8e => 185
	i32 3901907137, ; 635: Microsoft.VisualBasic.Core.dll => 0xe89260c1 => 3
	i32 3920810846, ; 636: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 45
	i32 3921031405, ; 637: Xamarin.AndroidX.VersionedParcelable.dll => 0xe9b630ed => 293
	i32 3928044579, ; 638: System.Xml.ReaderWriter => 0xea213423 => 157
	i32 3929187773, ; 639: Firebase.Storage.dll => 0xea32a5bd => 176
	i32 3930554604, ; 640: System.Security.Principal.dll => 0xea4780ec => 129
	i32 3931092270, ; 641: Xamarin.AndroidX.Navigation.UI => 0xea4fb52e => 278
	i32 3945713374, ; 642: System.Data.DataSetExtensions.dll => 0xeb2ecede => 24
	i32 3953953790, ; 643: System.Text.Encoding.CodePages => 0xebac8bfe => 134
	i32 3955647286, ; 644: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 236
	i32 3959773229, ; 645: Xamarin.AndroidX.Lifecycle.Process => 0xec05582d => 267
	i32 3971066695, ; 646: itext.styledxmlparser.dll => 0xecb1ab47 => 189
	i32 3977646153, ; 647: itext.io.dll => 0xed161049 => 183
	i32 3980434154, ; 648: th/Microsoft.Maui.Controls.resources.dll => 0xed409aea => 337
	i32 3987592930, ; 649: he/Microsoft.Maui.Controls.resources.dll => 0xedadd6e2 => 319
	i32 4003436829, ; 650: System.Diagnostics.Process.dll => 0xee9f991d => 30
	i32 4003906742, ; 651: HarfBuzzSharp.dll => 0xeea6c4b6 => 178
	i32 4015948917, ; 652: Xamarin.AndroidX.Annotation.Jvm.dll => 0xef5e8475 => 235
	i32 4024013275, ; 653: Firebase.Auth => 0xefd991db => 174
	i32 4025784931, ; 654: System.Memory => 0xeff49a63 => 63
	i32 4046471985, ; 655: Microsoft.Maui.Controls.Xaml.dll => 0xf1304331 => 206
	i32 4054681211, ; 656: System.Reflection.Emit.ILGeneration => 0xf1ad867b => 91
	i32 4066802364, ; 657: SkiaSharp.HarfBuzz => 0xf2667abc => 215
	i32 4068434129, ; 658: System.Private.Xml.Linq.dll => 0xf27f60d1 => 88
	i32 4073602200, ; 659: System.Threading.dll => 0xf2ce3c98 => 149
	i32 4091293555, ; 660: itext.forms.dll => 0xf3dc2f73 => 182
	i32 4094352644, ; 661: Microsoft.Maui.Essentials.dll => 0xf40add04 => 208
	i32 4099507663, ; 662: System.Drawing.dll => 0xf45985cf => 37
	i32 4100113165, ; 663: System.Private.Uri => 0xf462c30d => 87
	i32 4101593132, ; 664: Xamarin.AndroidX.Emoji2 => 0xf479582c => 256
	i32 4102112229, ; 665: pt/Microsoft.Maui.Controls.resources.dll => 0xf48143e5 => 332
	i32 4125707920, ; 666: ms/Microsoft.Maui.Controls.resources.dll => 0xf5e94e90 => 327
	i32 4126470640, ; 667: Microsoft.Extensions.DependencyInjection => 0xf5f4f1f0 => 196
	i32 4127667938, ; 668: System.IO.FileSystem.Watcher => 0xf60736e2 => 51
	i32 4130442656, ; 669: System.AppContext => 0xf6318da0 => 7
	i32 4147896353, ; 670: System.Reflection.Emit.ILGeneration.dll => 0xf73be021 => 91
	i32 4150914736, ; 671: uk\Microsoft.Maui.Controls.resources => 0xf769eeb0 => 339
	i32 4151237749, ; 672: System.Core => 0xf76edc75 => 22
	i32 4159265925, ; 673: System.Xml.XmlSerializer => 0xf7e95c85 => 163
	i32 4161255271, ; 674: System.Reflection.TypeExtensions => 0xf807b767 => 97
	i32 4164802419, ; 675: System.IO.FileSystem.Watcher.dll => 0xf83dd773 => 51
	i32 4181095122, ; 676: VersOne.Epub.dll => 0xf93672d2 => 226
	i32 4181436372, ; 677: System.Runtime.Serialization.Primitives => 0xf93ba7d4 => 114
	i32 4182413190, ; 678: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 272
	i32 4185676441, ; 679: System.Security => 0xf97c5a99 => 131
	i32 4186523351, ; 680: itext.svg.dll => 0xf98946d7 => 190
	i32 4196529839, ; 681: System.Net.WebClient.dll => 0xfa21f6af => 77
	i32 4213026141, ; 682: System.Diagnostics.DiagnosticSource.dll => 0xfb1dad5d => 28
	i32 4221941870, ; 683: Syncfusion.Pdf.Portable => 0xfba5b86e => 221
	i32 4252596737, ; 684: en/AppProyectoFinal.resources.dll => 0xfd797a01 => 0
	i32 4256097574, ; 685: Xamarin.AndroidX.Core.Core.Ktx => 0xfdaee526 => 249
	i32 4258378803, ; 686: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 0xfdd1b433 => 271
	i32 4260525087, ; 687: System.Buffers => 0xfdf2741f => 8
	i32 4271975918, ; 688: Microsoft.Maui.Controls.dll => 0xfea12dee => 205
	i32 4274976490, ; 689: System.Runtime.Numerics => 0xfecef6ea => 111
	i32 4292120959, ; 690: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 272
	i32 4294763496 ; 691: Xamarin.AndroidX.ExifInterface.dll => 0xfffce3e8 => 258
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [692 x i32] [
	i32 69, ; 0
	i32 68, ; 1
	i32 109, ; 2
	i32 198, ; 3
	i32 268, ; 4
	i32 302, ; 5
	i32 49, ; 6
	i32 211, ; 7
	i32 81, ; 8
	i32 146, ; 9
	i32 179, ; 10
	i32 210, ; 11
	i32 31, ; 12
	i32 343, ; 13
	i32 125, ; 14
	i32 209, ; 15
	i32 103, ; 16
	i32 286, ; 17
	i32 108, ; 18
	i32 286, ; 19
	i32 140, ; 20
	i32 306, ; 21
	i32 78, ; 22
	i32 125, ; 23
	i32 14, ; 24
	i32 242, ; 25
	i32 133, ; 26
	i32 288, ; 27
	i32 152, ; 28
	i32 340, ; 29
	i32 341, ; 30
	i32 19, ; 31
	i32 240, ; 32
	i32 27, ; 33
	i32 262, ; 34
	i32 2, ; 35
	i32 60, ; 36
	i32 43, ; 37
	i32 92, ; 38
	i32 245, ; 39
	i32 148, ; 40
	i32 264, ; 41
	i32 261, ; 42
	i32 312, ; 43
	i32 55, ; 44
	i32 70, ; 45
	i32 340, ; 46
	i32 231, ; 47
	i32 84, ; 48
	i32 325, ; 49
	i32 263, ; 50
	i32 324, ; 51
	i32 132, ; 52
	i32 56, ; 53
	i32 150, ; 54
	i32 75, ; 55
	i32 146, ; 56
	i32 63, ; 57
	i32 147, ; 58
	i32 175, ; 59
	i32 345, ; 60
	i32 166, ; 61
	i32 336, ; 62
	i32 246, ; 63
	i32 13, ; 64
	i32 259, ; 65
	i32 126, ; 66
	i32 153, ; 67
	i32 114, ; 68
	i32 167, ; 69
	i32 165, ; 70
	i32 261, ; 71
	i32 274, ; 72
	i32 85, ; 73
	i32 323, ; 74
	i32 317, ; 75
	i32 203, ; 76
	i32 214, ; 77
	i32 151, ; 78
	i32 306, ; 79
	i32 61, ; 80
	i32 199, ; 81
	i32 52, ; 82
	i32 104, ; 83
	i32 115, ; 84
	i32 41, ; 85
	i32 299, ; 86
	i32 297, ; 87
	i32 121, ; 88
	i32 212, ; 89
	i32 331, ; 90
	i32 53, ; 91
	i32 45, ; 92
	i32 185, ; 93
	i32 225, ; 94
	i32 120, ; 95
	i32 251, ; 96
	i32 329, ; 97
	i32 257, ; 98
	i32 82, ; 99
	i32 137, ; 100
	i32 293, ; 101
	i32 238, ; 102
	i32 9, ; 103
	i32 74, ; 104
	i32 311, ; 105
	i32 156, ; 106
	i32 308, ; 107
	i32 155, ; 108
	i32 93, ; 109
	i32 303, ; 110
	i32 46, ; 111
	i32 326, ; 112
	i32 314, ; 113
	i32 307, ; 114
	i32 110, ; 115
	i32 130, ; 116
	i32 26, ; 117
	i32 228, ; 118
	i32 73, ; 119
	i32 56, ; 120
	i32 344, ; 121
	i32 47, ; 122
	i32 335, ; 123
	i32 215, ; 124
	i32 202, ; 125
	i32 252, ; 126
	i32 23, ; 127
	i32 266, ; 128
	i32 224, ; 129
	i32 87, ; 130
	i32 44, ; 131
	i32 161, ; 132
	i32 72, ; 133
	i32 279, ; 134
	i32 4, ; 135
	i32 43, ; 136
	i32 64, ; 137
	i32 17, ; 138
	i32 54, ; 139
	i32 183, ; 140
	i32 338, ; 141
	i32 302, ; 142
	i32 106, ; 143
	i32 211, ; 144
	i32 307, ; 145
	i32 300, ; 146
	i32 263, ; 147
	i32 180, ; 148
	i32 35, ; 149
	i32 159, ; 150
	i32 86, ; 151
	i32 33, ; 152
	i32 13, ; 153
	i32 52, ; 154
	i32 57, ; 155
	i32 226, ; 156
	i32 283, ; 157
	i32 37, ; 158
	i32 197, ; 159
	i32 313, ; 160
	i32 301, ; 161
	i32 236, ; 162
	i32 36, ; 163
	i32 59, ; 164
	i32 270, ; 165
	i32 177, ; 166
	i32 182, ; 167
	i32 184, ; 168
	i32 18, ; 169
	i32 304, ; 170
	i32 165, ; 171
	i32 193, ; 172
	i32 326, ; 173
	i32 269, ; 174
	i32 201, ; 175
	i32 296, ; 176
	i32 332, ; 177
	i32 154, ; 178
	i32 292, ; 179
	i32 277, ; 180
	i32 330, ; 181
	i32 238, ; 182
	i32 30, ; 183
	i32 53, ; 184
	i32 187, ; 185
	i32 328, ; 186
	i32 297, ; 187
	i32 190, ; 188
	i32 191, ; 189
	i32 6, ; 190
	i32 312, ; 191
	i32 287, ; 192
	i32 219, ; 193
	i32 291, ; 194
	i32 243, ; 195
	i32 308, ; 196
	i32 235, ; 197
	i32 186, ; 198
	i32 254, ; 199
	i32 223, ; 200
	i32 86, ; 201
	i32 296, ; 202
	i32 62, ; 203
	i32 113, ; 204
	i32 58, ; 205
	i32 342, ; 206
	i32 283, ; 207
	i32 100, ; 208
	i32 20, ; 209
	i32 247, ; 210
	i32 112, ; 211
	i32 102, ; 212
	i32 103, ; 213
	i32 310, ; 214
	i32 105, ; 215
	i32 300, ; 216
	i32 72, ; 217
	i32 39, ; 218
	i32 33, ; 219
	i32 104, ; 220
	i32 74, ; 221
	i32 316, ; 222
	i32 10, ; 223
	i32 124, ; 224
	i32 47, ; 225
	i32 237, ; 226
	i32 203, ; 227
	i32 10, ; 228
	i32 44, ; 229
	i32 5, ; 230
	i32 181, ; 231
	i32 284, ; 232
	i32 320, ; 233
	i32 315, ; 234
	i32 32, ; 235
	i32 139, ; 236
	i32 93, ; 237
	i32 94, ; 238
	i32 335, ; 239
	i32 50, ; 240
	i32 142, ; 241
	i32 113, ; 242
	i32 141, ; 243
	i32 253, ; 244
	i32 116, ; 245
	i32 1, ; 246
	i32 301, ; 247
	i32 158, ; 248
	i32 77, ; 249
	i32 213, ; 250
	i32 80, ; 251
	i32 273, ; 252
	i32 38, ; 253
	i32 217, ; 254
	i32 295, ; 255
	i32 257, ; 256
	i32 250, ; 257
	i32 65, ; 258
	i32 139, ; 259
	i32 16, ; 260
	i32 117, ; 261
	i32 289, ; 262
	i32 298, ; 263
	i32 245, ; 264
	i32 186, ; 265
	i32 49, ; 266
	i32 71, ; 267
	i32 81, ; 268
	i32 127, ; 269
	i32 95, ; 270
	i32 122, ; 271
	i32 305, ; 272
	i32 222, ; 273
	i32 27, ; 274
	i32 266, ; 275
	i32 98, ; 276
	i32 29, ; 277
	i32 241, ; 278
	i32 333, ; 279
	i32 311, ; 280
	i32 150, ; 281
	i32 170, ; 282
	i32 5, ; 283
	i32 99, ; 284
	i32 34, ; 285
	i32 94, ; 286
	i32 288, ; 287
	i32 199, ; 288
	i32 22, ; 289
	i32 42, ; 290
	i32 171, ; 291
	i32 327, ; 292
	i32 259, ; 293
	i32 319, ; 294
	i32 273, ; 295
	i32 304, ; 296
	i32 298, ; 297
	i32 278, ; 298
	i32 3, ; 299
	i32 135, ; 300
	i32 112, ; 301
	i32 200, ; 302
	i32 339, ; 303
	i32 228, ; 304
	i32 336, ; 305
	i32 59, ; 306
	i32 96, ; 307
	i32 318, ; 308
	i32 40, ; 309
	i32 239, ; 310
	i32 26, ; 311
	i32 95, ; 312
	i32 193, ; 313
	i32 90, ; 314
	i32 100, ; 315
	i32 11, ; 316
	i32 88, ; 317
	i32 101, ; 318
	i32 222, ; 319
	i32 285, ; 320
	i32 194, ; 321
	i32 305, ; 322
	i32 230, ; 323
	i32 315, ; 324
	i32 8, ; 325
	i32 270, ; 326
	i32 310, ; 327
	i32 227, ; 328
	i32 89, ; 329
	i32 187, ; 330
	i32 265, ; 331
	i32 155, ; 332
	i32 314, ; 333
	i32 34, ; 334
	i32 117, ; 335
	i32 83, ; 336
	i32 21, ; 337
	i32 12, ; 338
	i32 163, ; 339
	i32 4, ; 340
	i32 207, ; 341
	i32 322, ; 342
	i32 202, ; 343
	i32 200, ; 344
	i32 85, ; 345
	i32 198, ; 346
	i32 309, ; 347
	i32 65, ; 348
	i32 324, ; 349
	i32 174, ; 350
	i32 292, ; 351
	i32 144, ; 352
	i32 274, ; 353
	i32 158, ; 354
	i32 42, ; 355
	i32 118, ; 356
	i32 195, ; 357
	i32 229, ; 358
	i32 318, ; 359
	i32 281, ; 360
	i32 132, ; 361
	i32 76, ; 362
	i32 67, ; 363
	i32 328, ; 364
	i32 173, ; 365
	i32 233, ; 366
	i32 144, ; 367
	i32 107, ; 368
	i32 152, ; 369
	i32 71, ; 370
	i32 220, ; 371
	i32 218, ; 372
	i32 157, ; 373
	i32 194, ; 374
	i32 122, ; 375
	i32 128, ; 376
	i32 323, ; 377
	i32 153, ; 378
	i32 256, ; 379
	i32 142, ; 380
	i32 243, ; 381
	i32 320, ; 382
	i32 213, ; 383
	i32 21, ; 384
	i32 15, ; 385
	i32 136, ; 386
	i32 76, ; 387
	i32 60, ; 388
	i32 246, ; 389
	i32 168, ; 390
	i32 169, ; 391
	i32 205, ; 392
	i32 16, ; 393
	i32 75, ; 394
	i32 7, ; 395
	i32 24, ; 396
	i32 268, ; 397
	i32 227, ; 398
	i32 92, ; 399
	i32 321, ; 400
	i32 2, ; 401
	i32 181, ; 402
	i32 137, ; 403
	i32 184, ; 404
	i32 269, ; 405
	i32 291, ; 406
	i32 135, ; 407
	i32 70, ; 408
	i32 147, ; 409
	i32 330, ; 410
	i32 309, ; 411
	i32 260, ; 412
	i32 201, ; 413
	i32 89, ; 414
	i32 97, ; 415
	i32 250, ; 416
	i32 255, ; 417
	i32 218, ; 418
	i32 325, ; 419
	i32 32, ; 420
	i32 46, ; 421
	i32 264, ; 422
	i32 0, ; 423
	i32 229, ; 424
	i32 110, ; 425
	i32 159, ; 426
	i32 36, ; 427
	i32 23, ; 428
	i32 115, ; 429
	i32 58, ; 430
	i32 289, ; 431
	i32 145, ; 432
	i32 119, ; 433
	i32 121, ; 434
	i32 111, ; 435
	i32 231, ; 436
	i32 140, ; 437
	i32 237, ; 438
	i32 55, ; 439
	i32 106, ; 440
	i32 331, ; 441
	i32 206, ; 442
	i32 207, ; 443
	i32 134, ; 444
	i32 303, ; 445
	i32 294, ; 446
	i32 282, ; 447
	i32 337, ; 448
	i32 260, ; 449
	i32 216, ; 450
	i32 210, ; 451
	i32 209, ; 452
	i32 160, ; 453
	i32 316, ; 454
	i32 247, ; 455
	i32 164, ; 456
	i32 133, ; 457
	i32 282, ; 458
	i32 162, ; 459
	i32 329, ; 460
	i32 271, ; 461
	i32 141, ; 462
	i32 294, ; 463
	i32 290, ; 464
	i32 170, ; 465
	i32 208, ; 466
	i32 220, ; 467
	i32 232, ; 468
	i32 299, ; 469
	i32 41, ; 470
	i32 258, ; 471
	i32 82, ; 472
	i32 57, ; 473
	i32 38, ; 474
	i32 98, ; 475
	i32 167, ; 476
	i32 188, ; 477
	i32 173, ; 478
	i32 216, ; 479
	i32 295, ; 480
	i32 83, ; 481
	i32 234, ; 482
	i32 99, ; 483
	i32 31, ; 484
	i32 160, ; 485
	i32 19, ; 486
	i32 128, ; 487
	i32 120, ; 488
	i32 254, ; 489
	i32 285, ; 490
	i32 267, ; 491
	i32 287, ; 492
	i32 166, ; 493
	i32 262, ; 494
	i32 345, ; 495
	i32 284, ; 496
	i32 275, ; 497
	i32 171, ; 498
	i32 17, ; 499
	i32 145, ; 500
	i32 322, ; 501
	i32 126, ; 502
	i32 119, ; 503
	i32 39, ; 504
	i32 116, ; 505
	i32 48, ; 506
	i32 143, ; 507
	i32 118, ; 508
	i32 189, ; 509
	i32 35, ; 510
	i32 177, ; 511
	i32 96, ; 512
	i32 54, ; 513
	i32 276, ; 514
	i32 130, ; 515
	i32 223, ; 516
	i32 154, ; 517
	i32 25, ; 518
	i32 162, ; 519
	i32 253, ; 520
	i32 149, ; 521
	i32 105, ; 522
	i32 212, ; 523
	i32 90, ; 524
	i32 241, ; 525
	i32 61, ; 526
	i32 143, ; 527
	i32 101, ; 528
	i32 6, ; 529
	i32 14, ; 530
	i32 123, ; 531
	i32 136, ; 532
	i32 29, ; 533
	i32 317, ; 534
	i32 73, ; 535
	i32 251, ; 536
	i32 25, ; 537
	i32 175, ; 538
	i32 214, ; 539
	i32 239, ; 540
	i32 180, ; 541
	i32 280, ; 542
	i32 277, ; 543
	i32 334, ; 544
	i32 138, ; 545
	i32 232, ; 546
	i32 248, ; 547
	i32 169, ; 548
	i32 281, ; 549
	i32 313, ; 550
	i32 102, ; 551
	i32 124, ; 552
	i32 252, ; 553
	i32 344, ; 554
	i32 196, ; 555
	i32 164, ; 556
	i32 168, ; 557
	i32 221, ; 558
	i32 255, ; 559
	i32 40, ; 560
	i32 204, ; 561
	i32 321, ; 562
	i32 18, ; 563
	i32 217, ; 564
	i32 172, ; 565
	i32 334, ; 566
	i32 333, ; 567
	i32 138, ; 568
	i32 151, ; 569
	i32 244, ; 570
	i32 156, ; 571
	i32 131, ; 572
	i32 20, ; 573
	i32 66, ; 574
	i32 219, ; 575
	i32 148, ; 576
	i32 48, ; 577
	i32 341, ; 578
	i32 192, ; 579
	i32 230, ; 580
	i32 80, ; 581
	i32 62, ; 582
	i32 107, ; 583
	i32 279, ; 584
	i32 192, ; 585
	i32 234, ; 586
	i32 188, ; 587
	i32 50, ; 588
	i32 265, ; 589
	i32 338, ; 590
	i32 276, ; 591
	i32 15, ; 592
	i32 176, ; 593
	i32 195, ; 594
	i32 69, ; 595
	i32 172, ; 596
	i32 240, ; 597
	i32 244, ; 598
	i32 191, ; 599
	i32 224, ; 600
	i32 343, ; 601
	i32 79, ; 602
	i32 249, ; 603
	i32 109, ; 604
	i32 233, ; 605
	i32 275, ; 606
	i32 225, ; 607
	i32 68, ; 608
	i32 64, ; 609
	i32 28, ; 610
	i32 161, ; 611
	i32 242, ; 612
	i32 11, ; 613
	i32 178, ; 614
	i32 204, ; 615
	i32 12, ; 616
	i32 179, ; 617
	i32 79, ; 618
	i32 127, ; 619
	i32 1, ; 620
	i32 84, ; 621
	i32 197, ; 622
	i32 67, ; 623
	i32 108, ; 624
	i32 66, ; 625
	i32 129, ; 626
	i32 123, ; 627
	i32 78, ; 628
	i32 290, ; 629
	i32 280, ; 630
	i32 342, ; 631
	i32 9, ; 632
	i32 248, ; 633
	i32 185, ; 634
	i32 3, ; 635
	i32 45, ; 636
	i32 293, ; 637
	i32 157, ; 638
	i32 176, ; 639
	i32 129, ; 640
	i32 278, ; 641
	i32 24, ; 642
	i32 134, ; 643
	i32 236, ; 644
	i32 267, ; 645
	i32 189, ; 646
	i32 183, ; 647
	i32 337, ; 648
	i32 319, ; 649
	i32 30, ; 650
	i32 178, ; 651
	i32 235, ; 652
	i32 174, ; 653
	i32 63, ; 654
	i32 206, ; 655
	i32 91, ; 656
	i32 215, ; 657
	i32 88, ; 658
	i32 149, ; 659
	i32 182, ; 660
	i32 208, ; 661
	i32 37, ; 662
	i32 87, ; 663
	i32 256, ; 664
	i32 332, ; 665
	i32 327, ; 666
	i32 196, ; 667
	i32 51, ; 668
	i32 7, ; 669
	i32 91, ; 670
	i32 339, ; 671
	i32 22, ; 672
	i32 163, ; 673
	i32 97, ; 674
	i32 51, ; 675
	i32 226, ; 676
	i32 114, ; 677
	i32 272, ; 678
	i32 131, ; 679
	i32 190, ; 680
	i32 77, ; 681
	i32 28, ; 682
	i32 221, ; 683
	i32 0, ; 684
	i32 249, ; 685
	i32 271, ; 686
	i32 8, ; 687
	i32 205, ; 688
	i32 111, ; 689
	i32 272, ; 690
	i32 258 ; 691
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!"Xamarin.Android remotes/origin/release/8.0.2xx @ 96b6bb65e8736e45180905177aa343f0e1854ea3"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"min_enum_size", i32 4}
