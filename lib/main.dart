// Copyright 2019 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

import 'package:asc_time_tracker/models/time_log_model.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:sizer/sizer.dart';
import 'package:google_fonts/google_fonts.dart';

import 'package:asc_time_tracker/utils/constants/theming.dart';
import 'package:asc_time_tracker/screens/screens.dart';

void main() {
  runApp(ChangeNotifierProvider(
    create: (context) => TimeLogModel(),
    child: MyApp(),
  ));
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Sizer(builder: (context, orientation, deviceType) {
      return MaterialApp(
        title: 'Flutter Demo',
        theme: ThemeData.dark().copyWith(
          scaffoldBackgroundColor: bgColor,
          textTheme: GoogleFonts.poppinsTextTheme(Theme.of(context).textTheme)
              .apply(bodyColor: bodyColor, displayColor: titleColor),
          canvasColor: secondaryColor,
        ),
        initialRoute: '/',
        routes: {
          '/': (context) => HomeScreen(),
          '/start': (context) => JobStartScreen(),
          '/list': (context) => JobListScreen(),
        },
      );
    });
  }
}
