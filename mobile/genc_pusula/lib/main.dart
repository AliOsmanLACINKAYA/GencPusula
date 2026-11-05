import 'dart:async';

import 'package:flutter/material.dart';

import 'app.dart';
import 'core/app_state.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  unawaited(appState.baslat());
  runApp(const GencPusulaApp());
}
