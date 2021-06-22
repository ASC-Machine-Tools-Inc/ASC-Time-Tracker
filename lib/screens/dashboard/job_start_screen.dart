import 'package:flutter/material.dart';

class JobStartScreen extends StatefulWidget {
  const JobStartScreen({Key? key}) : super(key: key);

  @override
  _JobStartScreenState createState() => _JobStartScreenState();
}

class _JobStartScreenState extends State<JobStartScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Time Page'),
      ),
    );
  }
}
