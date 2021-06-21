// Copyright 2019 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

import 'dart:async';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:sizer/sizer.dart';
import 'package:intl/intl.dart';

void main() {
  runApp(
    // Provide the model to all widgets within the app. We're using
    // ChangeNotifierProvider because that's a simple way to rebuild
    // widgets when a model changes. We could also just use
    // Provider, but then we would have to listen to Counter ourselves.
    //
    // Read Provider's docs to learn about all the available providers.
    ChangeNotifierProvider(
      // Initialize the model in the builder. That way, Provider
      // can own Counter's lifecycle, making sure to call `dispose`
      // when not needed anymore.
      create: (context) => Counter(),
      child: MyApp(),
    ),
  );
}

/// Simplest possible model, with just one field.
///
/// [ChangeNotifier] is a class in `flutter:foundation`. [Counter] does
/// _not_ depend on Provider.
class Counter with ChangeNotifier {
  int value = 0;

  void increment() {
    value += 1;
    notifyListeners();
  }
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Sizer(builder: (context, orientation, deviceType) {
      return MaterialApp(
          title: 'Flutter Demo',
          theme: ThemeData(
            primarySwatch: Colors.blue,
          ),
          initialRoute: '/',
          routes: {
            '/': (context) => MyHomePage(),
            '/start': (context) => StartJob(),
          });
    });
  }
}

class MyHomePage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Flutter Demo Home Page'),
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
                      const Text('You have pushed the button this many times:'),
                      // Consumer looks for an ancestor Provider widget
                      // and retrieves its model (Counter, in this case).
                      // Then it uses that model to build widgets, and will trigger
                      // rebuilds if the model is updated.
                      Consumer<Counter>(
                        builder: (context, counter, child) => Text(
                          '${counter.value}',
                          style: Theme.of(context).textTheme.headline4,
                        ),
                      ),
                    ],
                  ),
                ),
              ])),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          // You can access your providers anywhere you have access
          // to the context. One way is to use Provider.of<Counter>(context).
          //
          // The provider package also defines extension methods on context
          // itself. You can call context.watch<Counter>() in a build method
          // of any widget to access the current state of Counter, and to ask
          // Flutter to rebuild your widget anytime Counter changes.
          //
          // You can't use context.watch() outside build methods, because that
          // often leads to subtle bugs. Instead, you should use
          // context.read<Counter>(), which gets the current state
          // but doesn't ask Flutter for future rebuilds.
          //
          // Since we're in a callback that will be called whenever the user
          // taps the FloatingActionButton, we are not in the build method here.
          // We should use context.read().
          var counter = context.read<Counter>();
          counter.increment();
        },
        tooltip: 'Increment',
        child: const Icon(Icons.add),
      ),
    );
  }
}

class TimerText extends StatefulWidget {
  const TimerText({Key? key}) : super(key: key);

  @override
  _TimerTextState createState() => _TimerTextState();
}

class _TimerTextState extends State<TimerText> {
  String _now = DateFormat.yMd().add_jms().format(DateTime.now());
  Timer? _everySecond;

  @override
  void initState() {
    super.initState();

    // Defines a timer
    _everySecond = Timer.periodic(Duration(seconds: 1), (Timer t) {
      setState(() {
        // Gets the current time in format MM/dd/yyyy hh:mm:ss a
        // Example: 6/21/2021 3:18:46 PM
        _now = DateFormat.yMd().add_jms().format(DateTime.now());
      });
    });
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      child: new Text(_now, style: Theme.of(context).textTheme.subtitle1),
    );
  }
}

class StartJob extends StatefulWidget {
  const StartJob({Key? key}) : super(key: key);

  @override
  _StartJobState createState() => _StartJobState();
}

class _StartJobState extends State<StartJob> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Time Page'),
      ),
    );
  }
}
