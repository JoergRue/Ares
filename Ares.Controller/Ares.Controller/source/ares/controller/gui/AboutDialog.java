package ares.controller.gui;

import java.awt.FlowLayout;
import java.awt.Font;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.geom.AffineTransform;

import javax.swing.BorderFactory;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;

import ares.controller.gui.lf.BGDialog;
import ares.controller.util.Localization;

public class AboutDialog extends BGDialog {

	public AboutDialog(JFrame parent) {
		super(parent);
		initialize();
		setTitle(Localization.getString("AboutDialog.AboutTitle")); //$NON-NLS-1$
		setModal(true);
		setLocationRelativeTo(parent);
		getRootPane().setDefaultButton(getOKButton());
		setEscapeButton(getOKButton());
		pack();
	}
	
	private void initialize()
	{
		JPanel content = new JPanel();
		BoxLayout layout = new BoxLayout(content, BoxLayout.PAGE_AXIS);
		content.setLayout(layout);
		JLabel label1 = new JLabel(Localization.getString("AboutDialog.ARES")); //$NON-NLS-1$
		label1.setFont(getFont().deriveFont(AffineTransform.getScaleInstance(2.16, 2.16)).deriveFont(Font.BOLD));
		JPanel p1 = new JPanel(new FlowLayout(FlowLayout.CENTER));
		p1.add(label1);
		content.add(p1);
		content.add(Box.createVerticalStrut(10));
		JLabel label2 = new JLabel(Localization.getString("AboutDialog.ARESLong")); //$NON-NLS-1$
		label2.setFont(getFont().deriveFont(AffineTransform.getScaleInstance(1.73, 1.73)));
		JPanel p2 = new JPanel(new FlowLayout(FlowLayout.CENTER));
		p2.add(label2);
		content.add(p2);
		content.add(Box.createVerticalStrut(10));
		JPanel p3 = new JPanel(new FlowLayout(FlowLayout.CENTER));
		p3.add(new JLabel(Localization.getString("AboutDialog.Version") + ares.controller.control.Version.getCurrentVersionString())); //$NON-NLS-1$
		content.add(p3);
		JPanel p4 = new JPanel(new FlowLayout(FlowLayout.CENTER));
		p4.add(new JLabel(Localization.getString("AboutDialog.Copyright"))); //$NON-NLS-1$
		content.add(p4);
		content.add(Box.createVerticalStrut(10));
		JPanel p5 = new JPanel(new FlowLayout(FlowLayout.CENTER));
		p5.add(getOKButton());
		content.add(p5);
		content.setBorder(BorderFactory.createEmptyBorder(10, 20, 10, 20));
		this.setContentPane(content);
	}
	
	private JButton okButton;
	
	private JButton getOKButton() {
		if (okButton == null) {
			okButton = new JButton(Localization.getString("AboutDialog.OK")); //$NON-NLS-1$
			okButton.addActionListener(new ActionListener() {
				public void actionPerformed(ActionEvent e) {
					dispose();
				}
			});
		}
		return okButton;
	}
}
