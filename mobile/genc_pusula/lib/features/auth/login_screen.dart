import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../core/app_state.dart';
import '../../core/utils/validators.dart';
import '../../core/widgets/button.dart';
import '../../core/widgets/scaffold.dart';
import '../../core/widgets/text_field.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _email = TextEditingController();
  final _parola = TextEditingController();
  bool _gizliParola = true;
  bool _yukleniyor = false;

  @override
  void dispose() {
    _email.dispose();
    _parola.dispose();
    super.dispose();
  }

  Future<void> _girisYap() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _yukleniyor = true);
    try {
      await appState.girisYap(_email.text.trim(), _parola.text);
    } on ApiHatasi catch (e) {
      _hata(e.mesaj);
    } catch (_) {
      _hata('Giriş yapılamadı. Tekrar dene.');
    } finally {
      if (mounted) setState(() => _yukleniyor = false);
    }
  }

  void _hata(String mesaj) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(mesaj)));
  }

  @override
  Widget build(BuildContext context) {
    return AuthScaffold(
      title: 'Tekrar hoş geldin',
      subtitle: 'Fırsatları kaçırmamak için hesabına giriş yap.',
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppTextField(
              controller: _email,
              label: 'E-posta',
              hint: 'ornek@universite.edu.tr',
              prefixIcon: Icons.mail_outline,
              keyboardType: TextInputType.emailAddress,
              textInputAction: TextInputAction.next,
              validator: validateEmail,
            ),
            const SizedBox(height: 14),
            AppTextField(
              controller: _parola,
              label: 'Parola',
              prefixIcon: Icons.lock_outline,
              obscureText: _gizliParola,
              textInputAction: TextInputAction.done,
              onFieldSubmitted: (_) => _girisYap(),
              validator: validatePassword,
              suffixIcon: IconButton(
                onPressed: () {
                  setState(() => _gizliParola = !_gizliParola);
                },
                icon: Icon(
                  _gizliParola
                      ? Icons.visibility_outlined
                      : Icons.visibility_off_outlined,
                ),
              ),
            ),
            const SizedBox(height: 20),
            AppButton(
              label: 'Giriş yap',
              isLoading: _yukleniyor,
              onPressed: _girisYap,
            ),
            TextButton(
              onPressed: () => context.push('/register'),
              child: const Text('Hesabın yok mu? Kayıt ol'),
            ),
          ],
        ),
      ),
    );
  }
}
