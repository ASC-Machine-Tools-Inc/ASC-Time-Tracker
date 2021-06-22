import 'package:flutter/material.dart';

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
