import 'package:flutter/material.dart';
import 'package:sizer/sizer.dart';

import 'package:asc_time_tracker/widgets/timer.dart';
import 'package:asc_time_tracker/utils/constants.dart';

class HomeScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text(appTitle),
      ),
      body: Container(
          padding: const EdgeInsets.all(15),
          child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: <Widget>[
                Text(
                  'Hello, Test!',
                  style: Theme.of(context).textTheme.headline3,
                  textAlign: TextAlign.left,
                ),
                TimerText(),
                Divider(
                  height: 25,
                  thickness: 5,
                ),
                Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: <Widget>[
                      SizedBox(
                        width: 90.w,
                        height: 15.h,
                        child: GestureDetector(
                          onTap: () => Navigator.pushNamed(context, '/start'),
                          child: Card(
                              color: Colors.deepPurple,
                              child: Text('Start Time',
                                  style:
                                  Theme.of(context).textTheme.headline4)),
                        ),
                      ),
                    ],
                  ),
                ),
              ])),
    );
  }
}