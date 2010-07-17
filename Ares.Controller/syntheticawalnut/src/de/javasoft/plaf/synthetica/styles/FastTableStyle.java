package de.javasoft.plaf.synthetica.styles;

import javax.swing.JComponent;
import javax.swing.plaf.synth.Region;
import javax.swing.plaf.synth.SynthContext;
import javax.swing.plaf.synth.SynthStyle;

import de.javasoft.plaf.synthetica.styles.StyleWrapper;

public class FastTableStyle extends StyleWrapper {

  private FastTableStyle() {
  }
  
  public static FastTableStyle getStyle(SynthStyle style, JComponent c, Region region) {
    instance.setStyle(style);
    return instance;
  }
  
  public Object get(SynthContext sc, Object key) {
    return synthStyle.get(sc, key);
  }

  private static FastTableStyle instance = new FastTableStyle();

}
