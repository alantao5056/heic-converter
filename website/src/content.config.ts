import { defineCollection, z } from 'astro:content';
import { glob } from 'astro/loaders';

const articles = defineCollection({
  loader: glob({ pattern: '**/*.md', base: './src/content/articles' }),
  schema: z.object({
    title: z.string(),
    description: z.string(),
    pubDate: z.coerce.date(),
    updatedDate: z.coerce.date().optional(),
    /** Short direct answer rendered in the TL;DR box and cited by AI engines */
    tldr: z.string(),
    /** Rendered as an FAQ section and emitted as FAQPage JSON-LD */
    faq: z
      .array(z.object({ q: z.string(), a: z.string() }))
      .default([]),
    /** Optional HowTo JSON-LD for step-by-step guides */
    howto: z
      .object({
        name: z.string(),
        steps: z.array(z.object({ name: z.string(), text: z.string() })),
      })
      .optional(),
  }),
});

export const collections = { articles };
